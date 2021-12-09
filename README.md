# UnityNuget
Easy to use!No gimmick!One script!

傻瓜式Nuget管理工具

# How to use
1.Drop the Nuget to Assets

2.Restore all your nuget packages in Visual Studio/Rider ...

3.Open UnityEditor,click Nuget->Restore!

# 使用方法
1.把Nuget文件夹放到Assets里面去

2.在Visual Studio/Rider等等IDE里面还原nuget包

3.打开UnityEditor，点Nuget->Restore！

# 如何自定义本代码

本代码只有200行不到，浏览一下就知道怎么改了，但还是啰嗦几句，节约点看代码的时间

UnityNuget工作原理：根据packages.config将/Assets/../Packages目录下的nuget包复制到/Assets/Plugins/Nuget目录下，粗暴且简单。

默认会选择netstandard2.0的包，如果想要导入一些不支持netstandard2.0的包，那么在UnityNuget.NetVerOrder添加你想要的版本即可。例如
    
            private static readonly string[] NetVerOrder =
        {
            "netstandard2.0",
            "netstandard1.0",
        };
