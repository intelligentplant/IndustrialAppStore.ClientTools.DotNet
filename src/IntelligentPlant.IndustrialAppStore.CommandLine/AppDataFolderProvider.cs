namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// <see cref="AppDataFolderProvider"/> defines the base folder that will be used to store 
    /// application data such as authentication tokens.
    /// </summary>
    public class AppDataFolderProvider {

        /// <summary>
        /// The data folder.
        /// </summary>
        public DirectoryInfo? AppDataFolder { get; }


        /// <summary>
        /// Creates a new <see cref="AppDataFolderProvider"/> instance.
        /// </summary>
        /// <param name="dataFolder">
        ///   The data folder.
        /// </param>
        public AppDataFolderProvider(DirectoryInfo? dataFolder) {
            AppDataFolder = dataFolder;
        }


        /// <summary>
        /// Creates a new <see cref="AppDataFolderProvider"/> instance from the specified path.
        /// </summary>
        /// <param name="dataFolderPath">
        ///   The data folder path.
        /// </param>
        /// <remarks>
        ///   If a relative path is specified, it will be made absolute relative to <see cref="Environment.SpecialFolder.LocalApplicationData"/> 
        ///   if <see cref="Environment.UserInteractive"/> is <see langword="true"/>, and relative 
        ///   to <see cref="Environment.SpecialFolder.CommonApplicationData"/> otherwise.
        /// </remarks>
        internal AppDataFolderProvider(string? dataFolderPath) {
            if (!string.IsNullOrWhiteSpace(dataFolderPath)) {
                var fullPath = Path.IsPathRooted(dataFolderPath)
                    ? dataFolderPath
                    : Path.Combine(
                        Environment.UserInteractive
                            ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                            : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        dataFolderPath);
                AppDataFolder = new DirectoryInfo(fullPath);
            }
        }

    }
}
