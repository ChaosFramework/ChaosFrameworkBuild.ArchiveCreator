using System.IO;
using System.Linq;
using ArchiveHash = ChaosFramework.IO.ChaosArchive.ArchiveHash;
using SysCol = System.Collections.Generic;

namespace ChaosFrameworkBuild.ArchiveCreator.Util
{
    class Hash
    {
        public static string CalculateHash(ChaosArchive task, string directory)
        {
            System.Func<string> calculateHash = () =>
            {
                SysCol.IEnumerable<string> files =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                    .Select(file => ChaosUtil.Platform.Paths.Normalization.NormalizeFullPath(file).Substring(directory.Length + 1))
                    ;

                using (ArchiveHash hash = ArchiveHash.GetHash(files, file => File.ReadAllBytes($"{directory}/{file}")))
                    return hash.ToString();
            };

            return MeasuredTask.Run(task, calculateHash, "Calculated Source Hash (took {0}sec).");
        }
    }
}
