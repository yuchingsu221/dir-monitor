
using Domain.Models.Export;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Util.Interface
{
    public interface IFileUtility
    {
        public ExportFile ExportFile<T>(IEnumerable<T> records, string fileName);
    }
}
