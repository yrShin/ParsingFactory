using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample_DataType
{
    public class ParsingFactory
    {
        private const string JSON = "json";
        private const string NAME = "name";
        private const string SIZE = "size";
        private const string TYPE = "type";
        private const string KEY = "key";
        private const string VALUE = "value";
        private const string STRING = "string";
        private const string SBYTE = "sbyte";
        private const string SHORT = "short";
        private const string INTEGER = "integer";
        private const string INT = "int";
        private const string LONG = "long";
        private const string FLOAT = "float";
        private const string DOUBLE = "double";
        //아래는 파싱 결과를 scichart에서 이용할 필드
        private const string UUUUUUUUUUU = "UUUUUUUUUUU";
        private const string VVVVVVVVVVV = "VVVVVVVVVVV";
        private const string X = "x";
        private const string Y = "y";
        private const string Z = "z";
        private const string A = "a";
        private const string B = "b";
        private const string C = "c";
        private const string AAAAAAAAAAA = "AAAAAAAAAAA";
        private const string BBBBBBBBBBB = "BBBBBBBBBBB";
        private const string CCCCCCCCCCC = "CCCCCCCCCCC";
        private const string DDDDDDDDDDD = "DDDDDDDDDDD";
        private const string EEEEEEEEEEE = "EEEEEEEEEEE";
        private const string FFFFFFFFFFF = "FFFFFFFFFFF";
        private const string GGGGGGGGGGG = "GGGGGGGGGGG";
        private const string HHHHHHHHHHH = "HHHHHHHHHHH";
        private const string IIIIIIIIIII = "IIIIIIIIIII";
        private const string JJJJJJJJJJJ = "JJJJJJJJJJJ";
        private const string KKKKKKKKKKK = "KKKKKKKKKKK";
        private const string LLLLLLLLLLL = "LLLLLLLLLLL";
        private const string MMMMMMMMMMM = "MMMMMMMMMMM";
        private const string NNNNNNNNNNN = "NNNNNNNNNNN";
        private const string OOOOOOOOOOO = "OOOOOOOOOOO";
        private const string PPPPPPPPPPP = "PPPPPPPPPPP";
        private const string QQQQQQQQQQQ = "QQQQQQQQQQQ";
        private const string RRRRRRRRRRR = "RRRRRRRRRRR";
        private const string SSSSSSSSSSS = "SSSSSSSSSSS";
        private const string TTTTTTTTTTT = "TTTTTTTTTTT";

        //파싱 기능 저장용 Map
        public Dictionary<string, Func<string, sbyte[], IDataType>> creationFunctions;

        //포맷 저장용 JObject
        public JObject formatJson;

        //파싱 결과를 담을 List
        public JArray jArrayResult = new JArray();

        //Scichart에 표시할 데이터만 보관할 JsonObject
        public JObject jObjectScichartData = new JObject();

        public ParsingFactory()
        {
            creationFunctions = new Dictionary<string, Func<string, sbyte[], IDataType>>();
            AddCreationFunction(STRING, StringCustom.getCreationFunction());
            AddCreationFunction(INTEGER, IntegerCustom.getCreationFunction());
            AddCreationFunction(SHORT, ShortCustom.getCreationFunction());
            AddCreationFunction(FLOAT, FloatCustom.getCreationFunction());
            AddCreationFunction(DOUBLE, DoubleCustom.getCreationFunction());
            AddCreationFunction(LONG, LongCustom.getCreationFunction());
            AddCreationFunction(SBYTE, SbyteCustom.getCreationFunction());
        }

        private void AddCreationFunction(string className, Func<string, sbyte[], IDataType> f)
        {
            creationFunctions.Add(className, f);
        }

        public void Create(string readFIle, out string scichartJson, string filePath)
        {
            FormatConfiguration(readFIle);

            ParsingDataFileToJson(out List<string> scichartKeyList, filePath);

            ConvertToScichartJson(scichartKeyList);

            scichartJson = jObjectScichartData.ToString();
        }

        private void FormatConfiguration(string readFile)
        {
            formatJson = JObject.Parse(readFile);
        }

        //dat파일 파싱
        private void ParsingDataFileToJson(out List<string> scichartKeyList, string filePath)
        {
            jArrayResult = new JArray();
            scichartKeyList = new List<string>();
            
            JArray jsonFormatArray = formatJson.Value<JArray>(JSON);
            IEnumerator enumerator = jsonFormatArray.GetEnumerator();

            scichartKeyList.Clear();
            while (enumerator.MoveNext())
            {
                JObject fieldFormatJson = enumerator.Current as JObject;
                string fieldName = fieldFormatJson.Value<string>(NAME);
                scichartKeyList.Add(fieldName);
            }
            enumerator.Reset();

            //전체 바이트 길이 중 1번에 해당하는 offset
            int obj_byte_length = 100;

            try
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    long lCurOffset = 0L;
                    while (true)
                    {
                        byte[] bytes = binaryReader.ReadBytes(obj_byte_length);
                        sbyte[] sBytes = Array.ConvertAll(bytes, b => unchecked((sbyte)b));
                        JObject rawDataModel;
                        Dictionary<string, object> nameAndTypeAndValueDictionary = new Dictionary<string, object>();

                        if (sBytes.Length <= 0) break;

                        int startPoint = 0;
                        rawDataModel = new JObject();
                        while (enumerator.MoveNext())
                        {
                            JObject fieldFormatJson = enumerator.Current as JObject;
                            string fieldName = fieldFormatJson.Value<string>(NAME);
                            int fieldSize = fieldFormatJson.Value<int>(SIZE);
                            string fieldType = fieldFormatJson.Value<string>(TYPE);

                            sbyte[] byteForParsing = new sbyte[fieldSize];
                            Array.Copy(sBytes, startPoint, byteForParsing, 0, byteForParsing.Length);

                            if (!creationFunctions.ContainsKey(fieldType.ToLower()))
                            {
                                MessageBox.Show("formatJson 파일에 Sbyte, Short, Integer, Long, Float, Double, String이 아닌 type이 있습니다.");
                                break;
                            }
                                
                            if (fieldType.ToLower().Equals(STRING)
                                || !(fieldName.Equals(UUUUUUUUUUU) || fieldName.Equals(VVVVVVVVVVV) || fieldName.Equals(X) || fieldName.Equals(Y) || fieldName.Equals(Z)
                                || fieldName.Equals(A) || fieldName.Equals(B) || fieldName.Equals(C) || fieldName.Equals(AAAAAAAAAAA) || fieldName.Equals(BBBBBBBBBBB)
                                || fieldName.Equals(CCCCCCCCCCC) || fieldName.Equals(DDDDDDDDDDD) || fieldName.Equals(EEEEEEEEEEE) || fieldName.Equals(FFFFFFFFFFF)
                                || fieldName.Equals(GGGGGGGGGGG) || fieldName.Equals(HHHHHHHHHHH) || fieldName.Equals(IIIIIIIIIII) || fieldName.Equals(JJJJJJJJJJJ)
                                || fieldName.Equals(KKKKKKKKKKK) || fieldName.Equals(LLLLLLLLLLL) || fieldName.Equals(MMMMMMMMMMM) || fieldName.Equals(NNNNNNNNNNN)
                                || fieldName.Equals(OOOOOOOOOOO) || fieldName.Equals(PPPPPPPPPPP) || fieldName.Equals(QQQQQQQQQQQ) || fieldName.Equals(RRRRRRRRRRR)
                                || fieldName.Equals(SSSSSSSSSSS) || fieldName.Equals(TTTTTTTTTTT)))
                            {
                                //타입이 string -> scichart에 보내지 않음
                                if (scichartKeyList.Contains(fieldName)) scichartKeyList.Remove(fieldName);
                            }
                            else if (fieldType.ToLower().Equals(SBYTE)
                                || fieldType.ToLower().Equals(SHORT)
                                || fieldType.ToLower().Equals(INTEGER)
                                || fieldType.ToLower().Equals(INT)
                                || fieldType.ToLower().Equals(LONG)
                                || fieldType.ToLower().Equals(FLOAT)
                                || fieldType.ToLower().Equals(DOUBLE))
                            {
                                int typeByteSize = 0;
                                //바이트 사이즈에 대한 유효성 검사
                                switch (fieldType.ToString().ToLower())
                                {
                                    case INTEGER: typeByteSize = sizeof(int); break;
                                    case INT: typeByteSize = sizeof(int); break;
                                    case SHORT: typeByteSize = sizeof(short); break;
                                    case FLOAT: typeByteSize = sizeof(float); break;
                                    case LONG: typeByteSize = sizeof(long); break;
                                    case DOUBLE: typeByteSize = sizeof(double); break;
                                    case SBYTE: typeByteSize = sizeof(sbyte); break;
                                    default:
                                        MessageBox.Show("formatJson파일에서 해당 항목의 Type 확인 요망 : " + fieldType);
                                        break;
                                }
                                if (Convert.ToInt32(fieldSize) != typeByteSize)
                                {
                                    MessageBox.Show("입력된 size와 해당 데이터타입의 size가 일치하지 않습니다.");
                                    break;
                                }
                            }
                            IDataType field = creationFunctions[fieldType.ToLower()].Invoke(fieldName.ToLower(), byteForParsing);
                            startPoint += byteForParsing.Length;
                            Dictionary<string, object> fieldTypeAndValueDictionary = new Dictionary<string, object>();
                            fieldTypeAndValueDictionary.Add(fieldType, field.GetValue());
                            nameAndTypeAndValueDictionary.Add(fieldName, fieldTypeAndValueDictionary);
                        }
                        rawDataModel = JObject.FromObject(nameAndTypeAndValueDictionary);

                        jArrayResult.Add(rawDataModel);
                        enumerator.Reset();
                        lCurOffset += obj_byte_length;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        //파싱된 결과를 scichart에 보낼 형식으로 변환
        private void ConvertToScichartJson(List<string> scichartKeyList)
        {
            IEnumerator enumerator = jArrayResult.GetEnumerator();
            //jObjectScichartData의 value에 들어갈 JArray
            Dictionary<string, JArray> valuesArrayDictionary = new Dictionary<string, JArray>();
            foreach (string name in scichartKeyList)
            {
                valuesArrayDictionary.Add(name, new JArray());
            }

            while (enumerator.MoveNext())
            {
                JObject rawDataModelJObject = enumerator.Current as JObject;
                Dictionary<string, JObject> rawDataModelDictionary = rawDataModelJObject.ToObject<Dictionary<string, JObject>>();
                foreach (string fieldName in scichartKeyList)
                {
                    if (rawDataModelDictionary.ContainsKey(fieldName))
                    {
                        JObject fieldJObject = rawDataModelDictionary[fieldName];
                        Dictionary<string, object> fieldDictionary = fieldJObject.ToObject<Dictionary<string, object>>();
                        string fieldType = new List<string>(fieldDictionary.Keys).First();
                        var fieldValue = fieldDictionary[fieldType];
                        valuesArrayDictionary[fieldName].Add(fieldValue);
                    }
                }
            }

            JArray valuesArray = new JArray();

            List<string> dictionaryKey = new List<string>(valuesArrayDictionary.Keys);
            if (scichartKeyList.Count == dictionaryKey.Count)
            {
                for (int i = 0; i < dictionaryKey.Count; i++)
                {
                    if (!scichartKeyList[i].Equals(dictionaryKey[i])) continue;

                    JArray value = valuesArrayDictionary[dictionaryKey[i]];
                    valuesArray.Add(value);
                }
            }
            jObjectScichartData = new JObject();
            jObjectScichartData.Add(KEY, JArray.FromObject(scichartKeyList));
            jObjectScichartData.Add(VALUE, valuesArray);
        }
    }
}
