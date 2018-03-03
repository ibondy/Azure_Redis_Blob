using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Redis_Example
{
    class CreateTestFile
    {
        public CreateTestFile()
        {}

        public async Task CreateFile(int rows = 1000000)
        {
            using (StreamWriter writer = File.AppendText(@"f:\mytestfile.txt"))
            {
               
                for(int i=1; i<= rows; i++)
                {
                    var guid = Guid.NewGuid();
                    var hello = "Hello World";
                    await writer.WriteLineAsync($"{guid},{hello}");
                }

            }
        }
    }
}
