using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Robops.Lib.Transparencia
{
    public class LeitorZipTransparencia
    {
        public event EventHandler<string> InicioLeituraArquivo;
        public string FilePath { get; }
        public Func<string, bool> ShouldProcessFile { get; set; }
        public bool IgnoreFirstLine { get; set; }

        public LeitorZipTransparencia(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public IEnumerable<string> ReadLines()
        {
            var zip = System.IO.Compression.ZipFile.Open(FilePath, System.IO.Compression.ZipArchiveMode.Read);

            foreach (var entry in zip.Entries)
            {
                if (ShouldProcessFile != null)
                {
                    if (!ShouldProcessFile(entry.Name)) continue;
                }

                InicioLeituraArquivo?.Invoke(this, entry.Name);

                using var sr = new StreamReader(entry.Open(), Encoding.UTF7);
                //pula o cabeçalho
                if(IgnoreFirstLine) sr.ReadLine();

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
