using System.IO;
using System.Linq;
using ChaosUtil.Platform.Paths;
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
                directory = Normalization.NormalizePath(directory);
                SysCol.IEnumerable<string> files =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                    .Select(file => Normalization.NormalizePath(file).Substring(directory.Length).TrimStart('/'))
                    ;

                using (ArchiveHash hash = ArchiveHash.GetHash(files, file => File.ReadAllBytes($"{directory}/{file}")))
                    return hash.ToString();
            };

            return MeasuredTask.Run(task, calculateHash, "Calculated Source Hash (took {0}sec).");
        }
    }
}
