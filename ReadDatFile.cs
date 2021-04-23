using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample_DataType
{
    public class ReadDatFile
    {
        public void StartOpenFile(out string fileData, string filePathOrWorkArray)
        {
            ReadClassSetting(out string readFIle);

            SetRawDataModel(readFIle, out string json, filePathOrWorkArray);

            fileData = json;
        }

        private static void ReadClassSetting(out string readFIle)
        {
            string currentPath = Environment.CurrentDirectory;
            string pathConfig = currentPath + "\\config\\";
            string filePath = pathConfig + "formatJson.json";
            DirectoryInfo configDir = new DirectoryInfo(pathConfig);
            FileInfo fileInfo = new FileInfo(filePath);
            readFIle = "";
            if (configDir.Exists || fileInfo.Exists)
            {
                readFIle = File.ReadAllText(filePath);
            }
            else
            {
                MessageBox.Show("파일 경로 확인");
                return;
            }
        }

        private static void SetRawDataModel(string readFIle, out string json, string filePathOrWorkArray)
        {
            //ParsingFactory.GetInstance().Create(readFIle, out string scichartJson);
            ParsingFactory parsingFactory = new ParsingFactory();
            parsingFactory.Create(readFIle, out string scichartJson, filePathOrWorkArray);
            json = scichartJson;
        }
    }
}
