# UnityNuget
Easy to use!No gimmick!One script!

傻瓜式Nuget管理工具

# How to use
1. Drop the Nuget to Assets
2. Restore all your nuget packages in Visual Studio/Rider/others
3. Open UnityEditor,click Nuget->Restore!UnityNuget will copy dll from project/packages to project/Assets/Plugin/Nuget.
4. Handle error from packages if need.

# 使用方法
1. 把Nuget文件夹放到Assets里面去
2. 在Visual Studio/Rider等等IDE里面还原nuget包
3. 打开UnityEditor，点Nuget->Restore！
4. 以上步骤可以自动处理90%的包，一些特殊的包需要特殊处理：
    - PrecompiledAssemblyException  
    &emsp;&emsp;这是由于某个导入的nuget包依赖了比Unity的C#运行时更新的库。在所有情况下（截至2021.12.29），使用老版本的库是安全而且不会导致crash的，因此只要关闭Unity版本验证即可（UnityNuget已自动关闭了版本验证选项）。由于UnityNuget并不识别当前Unity版本自带了哪些类库，因此会将新版本的类库复制进Unity工程中，此时会因为存在同名库而报错，例如
    
            PrecompiledAssemblyException: Multiple precompiled assemblies with the same name xxx included or the current platform. Only one assembly with the same name is allowed per platform.
        &ensp;&ensp;兼容方案：移除Assets/Plugin/Nuget/下同名库的dll即可
    - 依赖了不存在、不兼容的库  
    &emsp;&emsp;Unity的C#运行时是残缺的，而且某些库会报PlatformNotsupport，这种情况下需要考虑改代码、更换兼容库、改低版本而不是nuget一键导入。典型的MySQL在Nuget的官方库就会报PlatformNotsupport。Log4Net的库依赖了ConfigurationManager这个类，而这个类并不能在Unity中使用，需要修改Log4Net代码生成无依赖版本。

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

