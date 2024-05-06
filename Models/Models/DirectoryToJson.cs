using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Models.Models
{
    class DirectoryToJson : DirectoryClass
    {
        public DirectoryToJson(FileSystemInfo fsi, string[] skipScanDirs, string[] specifyExtensions, List<ErrorFileObject> errors)
        {
            Name = fsi.Name;
            Directories = new List<DirectoryClass>();
            Files = new List<FileToJson>();

            if ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                IsFolder = true;
                foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos())
                {
                    if (!skipScanDirs.Contains(f.FullName))
                    {
                        if ((f.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            Directories.Add(new DirectoryToJson(f, skipScanDirs, specifyExtensions, errors));
                        }
                        else
                        {
                            Files.Add(new FileToJson
                            {
                                Name = f.Name,
                                IsFolder = false,
                                CreationTime = f.CreationTime,
                                UpdatedTime = f.LastWriteTime,
                                Length = ((FileInfo)f).Length
                            });

                            //記錄不在白名單內之副檔名檔案
                            if (!specifyExtensions.Contains(f.Extension.ToLower()))
                            {
                                errors.Add(new ErrorFileObject
                                {
                                    DirectoryName = ((FileInfo)f).DirectoryName,
                                    Name = f.Name,
                                    CreationTime = f.CreationTime,
                                    Length = ((FileInfo)f).Length
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}