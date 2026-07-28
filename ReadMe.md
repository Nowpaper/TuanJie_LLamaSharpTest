# 介绍

Unity团结引擎中实现本地大语言模型NPC对话功能

创作者：Nowpaper

## 配套文件

> 网盘链接: https://pan.baidu.com/s/1jf-1IGoutQMKoTAvQmEnPw?pwd=5gaw 提取码: 5gaw

下载模型文件保存到`Assets\StreamingAssets`目录中

模型地址：`https://huggingface.co/QuantFactory/Qwen2.5-1.5B-Instruct-GGUF/tree/main`

or

从网盘配套文件中下载。

细节参看视频讲解：`https://www.bilibili.com/video/BV1RqKs61EJy/`

## 更换模型

修改代码或在Editor中配置读取文件的路径

`LocalLLMNPC.cs` 中 `modelFileName` 变量


## 自行搭建LLamaSharp

- 安装`NuGet`

- 通过`NuGet`安装`LLamaSharp`和`LLamaSharp.Backend.CPU`

    - 如果报错，请检查是否因为编译库文件名冲突，最简单的方法是删除掉和运行系统不一样的编译库

- 重启，并设置`LLamaSharp.Backend.CPU`的对应平台编译库属性中`Load on Startup`为`True

- 下载GGUF模型文件放到`Assets\StreamingAssets`目录中

- 启动场景 `SampleScene`

- 检查测试NPC的参数是否正确，修改你想要的配置

## 引擎使用

**团结引擎 1.8 或以上版本**

上述流程和代码，可以在Unity其他版本中使用，但工程无法直接打开，可参照视频流程在Unity其他版本中实现。

## 视频地址

`https://www.bilibili.com/video/BV1RqKs61EJy/`

## 粉丝群

QQ群：`109751331`

