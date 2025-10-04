using System.IO;

namespace ChaosFrameworkBuild.ArchiveCreator.Util
{
    class Archive
    {
        public static void CreateArchive(ChaosArchive task, string sourceDirectory, string targetFile)
        {
            System.Func<int> createArchive = () =>
            {
                ChaosFramework.IO.ChaosIO.Init();

                Directory.GetParent(targetFile).Create();
                ChaosFramework.IO.ChaosArchive.Tools.CreateArchive(
                    sourceDirectory,
                    Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories),
                    targetFile
                );

                return 0;
            };

            MeasuredTask.Run(task, createArchive, "Successfully built archive! (took {0}sec).");
        }
    }
}
