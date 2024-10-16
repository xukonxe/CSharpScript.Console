
# CSharpScript.Console

轻量级的交互式 C# 脚本控制台，用于实时执行 C# 代码。此项目基于 **C#Script**。在宿主程序中创建后，允许开发者在运行时使用原生 C# 语法与宿主程序动态交互，无需重新编译整个项目，以为快速原型开发和测试提供灵活性。

相关仓库：[CsharpScript.Console C#Script 运行时控制台](https://github.com/xukonxe/CSharpScript.Running)

## 特性

- **方便的控制台创建、自定义操作**  
  根据需求，可轻松创建和自定义控制台。参考Example.cs。

- **命名空间和程序集导入**  
  创建时导入命名空间、引用程序集。

- **全局变量定义与操作**  
  轻松定义和操作全局变量。

- **实时交互脚本执行**  
  实时执行脚本，提供即时反馈和错误报告。

- **直观的命令**  
  命令操作直观方便。例如使用 `[vars]` 来查询已定义的变量和方法。

## 使用场景

- **嵌入式 C# 控制台**  
  适用于需要嵌入 C# 控制台，在运行时操作宿主程序、执行代码片段或执行实时任务的开发者。

- **快速原型开发和测试**  
  动态运行代码，无需重新编译，提升开发和实验速度。

- **教育用途**  
  非常适合用于教授或学习 C# ，具有实时反馈和变量检查功能。

- **快速部署**
  适用于需要快速临时部署各种基于C#的服务、程序。

## 入门

使用 **CSharpScript.Console**，克隆此仓库，用法参考Example.cs。

```bash
git clone https://github.com/xukonxe/CSharpScript.Console.git
```

## 要求
- Microsoft.CodeAnalysis.CSharp
- Microsoft.CodeAnalysis.CSharp.Script

## 许可
**本项目基于 MIT 许可协议 - 详情请查看 LICENSE 文件。**
