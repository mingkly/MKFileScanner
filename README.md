# MKFileScanner
<p>file scanner in maui,need used with mkfilepicker nuget package.only support android and windows</p>
<p> To use this package normal,add MkFilePicker NugetPackage to you maui project.</p>
<p>为了正常使用该包，需要先安装MKFilePicker包到你的Maui项目上.</p>
<p> 1.Scan file via folder:</p>
<p>通过挑选的文件夹扫描</p>
<p>on windows, you must provider a folder to scan,you can use MkFilePicker nuget package to pick a folderer or get from Environment.getxxxfolder or somehting else</p>
<p>在windows上，你必须提供一个文件夹用来扫描，可以使用MkFilePicker挑选一个文件夹来扫描或者使用Environment相关api获取文件夹或者其他获得文件夹的方法</p>
<code>
    private async Task Scan()
    {
        var res = await MKFilePicker.MKFilePicker.PickFolderAsync(null).ConfigureAwait(false);
        if (res != null)
        {
            MKFileScanner.MKFileScanner.Scan(res.PlatformPath,null, OnFileScanned);
        }
    }
  
  
    async void OnFileScanned(FileResult fileResult)
    {
        //open scanned file
        //打开扫描的文件
        using (var stream = MKFilePicker.MKFilePicker.OpenPickedFile(fileResult.PlatformPath, "r"))
        {
            Debug.WriteLine($"{fileResult.FileName},{fileResult.PlatformPath},{fileResult.AbsolutePath},{fileResult.FileSize},{stream.Length}");
        }
        //read metadata of scanned file
        //读取扫描文件的元数据
        var map = await MKFileScanner.MKFileScanner.ReadMetadataAsync(fileResult.PlatformPath);
        if(map != null)
        {
            //map.Name
            //map.CreateTime
        }
        if (map  is AudioMetadataMap audio)
        {
            //audio.SampleRate
            //audio.Artist
        }
        else if(map is VideoMetadataMap video)
        {
            //video.Resolution
        }
        else if(map is ImageMetadataMap image)
        {
            //image.Width
            //image.Height
        }
    }
 </code>
 
 <p>2.scan without a folder</p>
 <p>直接扫描</p>
 <p>this only work on android,and you need take read/writeExternalStorgae permission</p>
 <p>只有安卓能这样做，而且你必须获得读写外部权限</p>
 <code>
        var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                //未获取到权限
            }
        }
        var options = new FileScanOption
        {
            FileType = FileType.Video,          
            //可以对文件名进行条件筛选，支持的语句将会转化为sql条件语句,不支持的将在扫描后过滤
            //FileNameExpression = s => s.StartsWith("S")
        };
        await Task.Run(() => MKFileScanner.MKFileScanner.Scan(null, options, OnFileScanned));
  </code>
  <p>you can set FileScanOption.FileType to set which collection you want to scan </p>
  <p>你可以设置FileType的值来设置你想扫描哪个集合</p>
  
  <p>3,scan file use option</p>
  <p>对扫描结果过滤</p>
  <p>limit file extension</p>
  <p>限制文件拓展名</p>
  <code>
        var options = new FileScanOption();
        options.Extensions = new string[] { "jpg", "png" };
  </code>
  <p>filter file name</p>
  <p>筛选文件名</p>
  <code>
    options.FileNameExpression = s => s.StartsWith("S");
  </code>
  <p>file size bigger than 1kB and create time less one day</p>
  <p>筛选文件大于1kB且一天以内创建的</p>
  <code>
        options.MinFileSize = 1000;
        options.MinCreateTime = DateTime.Now - TimeSpan.FromDays(1);
  </code>
  
