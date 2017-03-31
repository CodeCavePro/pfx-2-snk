using CommandLine;

namespace CodeCave.Utility.Windows.Pfx2Snk
{
    class Options
    {
        /// <summary>
        /// Gets or sets the input file.
        /// </summary>
        /// <value>
        /// The input file.
        /// </value>
        [Option('k', "key", Required = true, HelpText = "Input PFX file to be processed.")]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Option('p', "password", Required = false, HelpText = "Password for PFX file.")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        /// <value>
        /// The output file.
        /// </value>
        [Option('o', "output", Required = false, HelpText = "Output SNK file to be generated.")]
        public string OutputFile { get; set; }
    }
}
