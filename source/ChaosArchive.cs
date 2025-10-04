using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;

namespace ChaosFrameworkBuild.ArchiveCreator
{
    public class ChaosArchive : Task
    {
#pragma warning disable ChaosCC0102 // these are MSBuild properties, therefore use PascalCase

        /// <summary> The path of the ChaosArchive source folder relative to the project directory. </summary>
        [Required]
        public string SourceFolder { get; set; }

        /// <summary> The relative path of the ChaosArchive file inside the build folder. </summary>
        [Required]
        public string TargetFile { get; set; }

        /// <summary> The path of the build directory relative to the project directory. </summary>
        [Required]
        public string OutputPath { get; set; }

        /// <summary> The path of the intermediate build directory relative to the project directory. </summary>
        [Required]
        public string IntermediateOutputPath { get; set; }

#pragma warning restore ChaosCC0102

        string binArchiveFile => $"{OutputPath}\\{TargetFile}";
        string objArchiveHashFile => $"{IntermediateOutputPath}\\{TargetFile}.hash";

        public void LogMsg(string message) => Log.LogMessage(MessageImportance.High, $"-- {message}");

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Running ChaosArchiveCreator...");
            return Util.MeasuredTask.Run(this, CreateArchive, "ChaosArchiveCreator finished! (took {0}sec).");
        }

        bool CreateArchive()
        {
            LogMsg("Running in:  " + System.Environment.CurrentDirectory);
            LogMsg("Source Path: " + SourceFolder);
            LogMsg("Target Path: " + TargetFile);
            LogMsg("Output Path: " + OutputPath);
            LogMsg("TmpOut Path: " + IntermediateOutputPath);

            if (!Directory.Exists(SourceFolder))
            {
                Log.LogError("Source directory does not exist.");
                return false;
            }

            string fullSourceFolder = Path.GetFullPath(SourceFolder);
            string fullArchivePath = Path.GetFullPath(binArchiveFile);

            string srcHash = Util.Hash.CalculateHash(this, fullSourceFolder);
            string objHash = GetObjHash();
            LogMsg("Src Hash: " + srcHash);
            LogMsg("Obj Hash: " + objHash);

            if (srcHash == objHash && File.Exists(fullArchivePath))
            {
                LogMsg("No changes to archive detected.");
                LogMsg("Skipping archive creation.");
                return true;
            }

            LogMsg("Changes to archive detected.");
            LogMsg("Recreating archive...");
            Directory.GetParent(objArchiveHashFile).Create();
            File.WriteAllText(objArchiveHashFile, srcHash);

            Util.Archive.CreateArchive(this, fullSourceFolder, fullArchivePath);

            return true;
        }

        string GetObjHash()
        {
            string archiveHash = "<no hash file>";
            if (File.Exists(objArchiveHashFile))
                archiveHash = File.ReadAllLines(objArchiveHashFile)[0];

            return archiveHash;
        }
    }
}
