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

# How to block package
Because nuget references automatically reference dependency packages, some dependency packages may conflict with old packages of the same name that Unity brings with it, such as those under the Editor/Data/NetStandard.2.0.0 directory, and it is time for UnityNuget to ignore them
The simple solution is to manually delete the dll file for the package after restoring the Nuget package (e.g. System.Numerics.Vectors.dll).
When you restore later, you won't copy the dll here, and no additional configuration is required
# 如何拉黑包
由于nuget引用的时候会自动引用依赖包，一些依赖包可能会与Unity自己带的同名老包冲突，例如Editor\Data\NetStandard\Extensions\2.0.0目录下的包，这个时候就要让UnityNuget忽略这些包了
方法很简单，还原Nuget包后，手动删除对应包的dll文件即可（例如System.Numerics.Vectors.dll）。
以后还原的时候，就不会复制dll到这里了，不需要其他配置

# 如何自定义本代码

本代码只有200行不到，浏览一下就知道怎么改了，但还是啰嗦几句，节约点看代码的时间

UnityNuget工作原理：根据packages.config将/Assets/../Packages目录下的nuget包复制到/Assets/Plugins/Nuget目录下，粗暴且简单。另外为了避免导入包报错的时，UnityNuget不编译，因此添加了UnityNuget.asmdef，使UnityNuget不因主工程错误而不能使用。

默认会选择netstandard2.0的包，如果想要导入一些不支持netstandard2.0的包，那么在UnityNuget.NetVerOrder添加你想要的版本即可。例如
    
            private static readonly string[] NetVerOrder =
        {
            "netstandard2.0",
            "netstandard1.0",
        };

