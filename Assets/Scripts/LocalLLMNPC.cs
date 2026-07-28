using UnityEngine;
using LLama.Common;
using LLama;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using System.IO;

public class LocalLLMNPC : MonoBehaviour
{
    public string npcName = "老铁";
    // NPC 角色设定
    public string roleContext = "你是一个废土世界的雇佣兵，冷酷且不苟言笑。";

    // 模型配置的文件名，根据需要修改，记得在StreamingAssets中添加对应的文件
    string modelFileName = "qwen2.5-1.5b-instruct-q4_k_m.gguf";

    // 核心对象
    private LLamaWeights _modelWeights;
    private LLamaContext _context;
    private InteractiveExecutor _executor;

    // 新增：ChatSession，用于更方便地管理上下文和对话状态
    private ChatSession _chatSession;

    // 线程同步
    private SynchronizationContext _mainThreadContext;
    private CancellationTokenSource _cancelTokenSource;

    void Start()
    {
        _mainThreadContext = SynchronizationContext.Current;
        InitModelAsync();
    }

    private async void InitModelAsync()
    {
        var modelPath = Path.Combine(Application.streamingAssetsPath, modelFileName);
        Debug.Log("正在加载模型：" + modelPath);
        // 配置模型参数 (上下文窗口大小等)
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 2048, // 根据 NPC 记忆需求调整，越大越吃内存
            GpuLayerCount = 0   // 如果使用了 GPU 后端，这里填 99 即可将计算卸载到显卡
        };

        // 异步加载模型权重，避免卡顿
        await Task.Run(() =>
        {
            _modelWeights = LLamaWeights.LoadFromFile(parameters);
            _context = _modelWeights.CreateContext(parameters);
            _executor = new InteractiveExecutor(_context);

            // 初始化 ChatHistory 并注入 NPC 设定
            var chatHistory = new ChatHistory();
            string systemPrompt = $"{roleContext}你的名字叫{npcName}。";
            chatHistory.AddMessage(AuthorRole.System, systemPrompt);

            // 将 Executor 和 History 传入，构建 ChatSession
            _chatSession = new ChatSession(_executor, chatHistory);
        });

        Debug.Log("智能体模型加载完成！(ChatSession模式)");
    }

    // 暴露给游戏交互逻辑的调用接口
    public async Task SpeakToNPC(string playerInput, Text dialogueText)
    {
        if (_chatSession == null) return;

        dialogueText.text = ""; // 清空当前对话框
        _cancelTokenSource = new CancellationTokenSource();

        // 设定生成参数
        var inferenceParams = new InferenceParams()
        {
            MaxTokens = 256, // 限制单次回答长度
            AntiPrompts = new[] { "User:" } // 停止词，防止模型自说自话
        };

        // 在后台线程执行推理流
        await Task.Run(async () =>
        {
            // 将玩家输入包装为 ChatHistory.Message 丢入 ChatSession
            var message = new ChatHistory.Message(AuthorRole.User, playerInput);
            var chatStream = _chatSession.ChatAsync(message,
                                    inferenceParams, _cancelTokenSource.Token);
            await foreach (var token in chatStream)
            {
                // 将生成的每一个字推回 Unity 主线程更新 UI
                _mainThreadContext.Post(_ => { dialogueText.text += token; }, null);
            }

            // 生成结束后，清理可能漏掉的停止词
            _mainThreadContext.Post(_ =>
            {
                string finalText = dialogueText.text;
                // 仅清理末尾的停止词，防止误伤正文中的正常内容
                if (finalText.EndsWith("User:") || finalText.EndsWith("\nUser"))
                    finalText = finalText.Substring(0, finalText.Length - 5);

                dialogueText.text = finalText.Trim();
            }, null);
        });
    }

    void OnDestroy()
    {
        // ！！！极其重要：C++ 侧的非托管内存必须手动释放 ！！！
        _cancelTokenSource?.Cancel();
        _context?.Dispose();
        _modelWeights?.Dispose();
    }
}