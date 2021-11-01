using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace forexcel2
{

    class Program
    {
        static void Main(string[] args)
        {
            // 시트 이름 (복사용)
            List<string> sheetList = new List<string>()
            {
                "BuffCC_Table",
                "Enemy_Table",
                "Level_Table",
                "Prefab_Table",
                "Shop_Table",
                "SkillCondition_Table",
                "SkillStat_Table",
                "Icon_Table",
                "Stage_Table",
                "StageEnemy_Table",
                "Synergy_Table",
                "Tower_Table"
            };
            // 1. 경로\파일이름 설정 (파일 우클릭 - 속성 - 일반 탭 - 위치)
            // 예시 @"D:\TD\(ALL) DataTable_12.2.xlsx"
            string filepath = @"C:\Users\kimmk\source\repos\ExcelReader\Data\";
            string filename = "(ALL) DataTable_13.8.xlsx";
            // 2. 뽑고자 하는 테이블의 시트 이름 설정
            string sheet = "Tower_Table";
            // 3. Ctrl + f5 실행

            ExcelReader excel = new ExcelReader();
            excel.__Initialize(filepath + filename);
            excel.ReadSheet(sheet);
            excel.__Finalize();
        }
    }

    class ExcelReader
    {
        public struct Pos
        {
            public int row;
            public int col;

            public Pos(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }

        Application m_app = null;
        Workbook m_workbook = null;
        string filepath;

        bool IsFinalized = false;

        public ExcelReader()
        {
        }

        ~ExcelReader()
        {
            if (!IsFinalized)
                __Finalize();
        }

        public bool __Initialize(string path)
        {
            m_app = new _Excel.Application();
            m_workbook = m_app.Workbooks.Open(path);

            filepath = path;

            return true;
        }

        public void __Finalize()
        {
            IsFinalized = true;

            m_workbook.Close(false, Type.Missing, Type.Missing);
            m_app.Quit();

            DeleteObj(m_workbook);
            DeleteObj(m_app);

            GC.Collect();
        }

        public void DeleteObj(Object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
                Console.WriteLine("DeleteObj Error");
            }
            finally
            {
                GC.Collect();
            }
        }


        public void ReadSheet(string sheetName)
        {
            // sheet
            _Excel.Worksheet sheet = m_workbook.Worksheets.get_Item(sheetName);

            _Excel.HPageBreak hbreak_start = sheet.HPageBreaks[1];
            _Excel.VPageBreak vbreak_start = sheet.VPageBreaks[1];

            _Excel.HPageBreak hbreak_end = sheet.HPageBreaks[2];
            _Excel.VPageBreak vbreak_end = sheet.VPageBreaks[2];

            _Excel.Range vbreak_start_range = vbreak_start.Location;
            _Excel.Range vbreak_end_range = vbreak_end.Location;
            _Excel.Range hbreak_start_range = hbreak_start.Location;
            _Excel.Range hbreak_end_range = hbreak_end.Location;

            ToFile(sheet, vbreak_start_range.Column, vbreak_end_range.Column, hbreak_start_range.Row, hbreak_end_range.Row);

            DeleteObj(hbreak_start_range);
            DeleteObj(hbreak_end_range);
            DeleteObj(vbreak_start_range);
            DeleteObj(vbreak_end_range);
            DeleteObj(sheet);
            DeleteObj(hbreak_start);
            DeleteObj(vbreak_start);
            DeleteObj(hbreak_end);
            DeleteObj(vbreak_end);
        }


        void ToFile(_Excel.Worksheet sheet, int Left, int Right, int Top, int Bottom)
        {
            int idx = filepath.LastIndexOf('\\');
            string sheetName = sheet.Name;
            string structName = sheetName + "Excel";
            string savepath_cs = System.IO.Path.Combine(filepath.Substring(0, idx), structName + "Loader" + ".cs");
            string savepath_data = System.IO.Path.Combine(filepath.Substring(0, idx), sheetName + ".txt");
            StringBuilder sb = new StringBuilder();

            List<string> types = new List<string>();
            List<string> names = new List<string>();
            // 1. unity cs 파일 생성
            // struct 및 scriptable object 
            sb.Length = 0;

            // include file
            sb.Append("using System.Collections.Generic;\n");
            sb.Append("using UnityEngine;\n\n");
            // -- include end

            // data struct
            sb.Append("[System.Serializable]\n");
            sb.Append("public struct ");
            sb.Append(structName);
            sb.Append('\n');
            sb.Append("{\n");

            for (int j = Left; j < Right; j++)
            {   // 멤버 변수들
                _Excel.Range typename_range = sheet.Cells[Top + 1, j];
                _Excel.Range data_range = sheet.Cells[Top, j];

                string type_name = typename_range.Value2.ToString();
                string indata = data_range.Value2.ToString();
                types.Add(type_name);
                names.Add(indata);

                sb.Append("\tpublic ");
                sb.Append(type_name);
                sb.Append(' ');
                sb.Append(indata);
                sb.Append(";\n");

                DeleteObj(typename_range);
                DeleteObj(data_range);
            }

            sb.Append("}\n\n");
            // -- struct end

            sb.Append("\n\n");
            sb.Append("//////////////////////////");
            sb.Append("\n\n");

            // scriptable object (unity)
            // [CreateAssetMenu(fileName = "Tower Data", menuName = "Scriptable Object/TowerData")]
            sb.Append("[CreateAssetMenu(fileName = \"" + sheetName + "Loader" + "\", menuName = " + "\"Scriptable Object/" + sheetName + "Loader" + "\")]\n");
            sb.Append("public class  " + structName + "Loader " + ": ScriptableObject\n");
            sb.Append("{\n");

            string tab = "";
            {
                tab += '\t';
                // memeber variables
                sb.Append(tab + "[SerializeField] string filepath;" + '\n');
                sb.Append(tab + "public List<" + structName + "> DataList;\n\n");


                // read line function
                sb.Append(tab + "private " + structName + " Read(string line)\n");
                sb.Append(tab + "{\n");
                {
                    tab += '\t';

                    sb.Append(tab + "line = line.TrimStart('\\n');\n\n");

                    sb.Append(tab + structName + " data = new " + structName + "();\n");
                    sb.Append(tab + "int idx = 0;\n");
                    sb.Append(tab + "string[] strs = line.Split('`');\n\n");

                    for (int i = 0; i < names.Count; i++)
                    {   // member setting
                        // parsing
                        switch (types[i])
                        {
                            case "string":
                                sb.Append(tab + "data." + names[i] + " = strs[idx++];");
                                break;
                            default:
                                sb.Append(tab + "data." + names[i] + " = " + types[i] + ".Parse(strs[idx++]);");
                                break;
                        }
                        sb.Append('\n');
                    }
                    sb.Append('\n');
                    sb.Append(tab + "return data;\n");
                    tab = tab.Remove(tab.Length - 1);
                }
                sb.Append(tab + "}\n");


                // ReadAllFromFile function
                sb.Append(tab + "[ContextMenu(\"파일 읽기\")]\n");
                sb.Append(tab + "public void ReadAllFromFile()\n");
                sb.Append(tab + "{\n");
                {
                    tab += '\t';
                    sb.Append(tab + "DataList = new List<" + structName + ">();\n\n");

                    sb.Append(tab + "string currentpath = System.IO.Directory.GetCurrentDirectory();\n");
                    sb.Append(tab + "string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));\n");
                    sb.Append(tab + "string[] strs = allText.Split(';');\n\n");

                    sb.Append(tab + "foreach (var item in strs)\n");
                    sb.Append(tab + "{\n");
                    {
                        tab += '\t';
                        sb.Append(tab + "if (item.Length < 2)\n");
                        sb.Append(tab + '\t' + "continue;\n");
                        sb.Append(tab + structName + " data = Read(item);\n");
                        sb.Append(tab + "DataList.Add(data);\n");
                    }
                    tab = tab.Remove(tab.Length - 1);
                    sb.Append(tab + "}\n");
                }
                tab = tab.Remove(tab.Length - 1);
                sb.Append(tab + "}\n");
            }
            tab = tab.Remove(tab.Length - 1);
            sb.Append(tab + "}\n");

            // WARNING!! 읽어들이는 데이터 테이블에는 No(int)이 있다고 가정하고 작성함
            // GetData function
            // TODO : 추후 reflection 으로 수정할 것
            //sb.Append(tab + "public " + structName + "? GetData(int No)\n");
            //sb.Append(tab + "{\n");
            //{
            //    tab += '\t';

            //    sb.Append(tab + "foreach (var item in DataList)\n");
            //    sb.Append(tab + "{\n");
            //    {
            //        tab += '\t';

            //        sb.Append(tab + "if(item.No == No)\n");
            //        sb.Append(tab + "{\n");
            //        {
            //            tab += '\t';
            //            sb.Append(tab + "return item;\n");
            //            tab = tab.Remove(tab.Length - 1);
            //        }
            //        sb.Append(tab + "}\n");
            //        tab = tab.Remove(tab.Length - 1);
            //    }
            //    sb.Append(tab + "}\n");
            //    sb.Append(tab + "return null;\n");

            //    tab = tab.Remove(tab.Length - 1);
            //    sb.Append(tab + "}\n");
            //}
            //sb.Append("}\n");

            CreateFile(savepath_cs);
            SaveFile(savepath_cs, sb.ToString());



            // data file 생성
            sb.Length = 0;

            for (int i = Top + 2; i < Bottom; i++)
            {
                for (int j = Left; j < Right; j++)
                {
                    _Excel.Range r = sheet.Cells[i, j];
                    sb.Append(r.Value2.ToString());
                    sb.Append('`');
                    DeleteObj(r);
                }
                --sb.Length;
                sb.Append(';');
                sb.Append('\n');
            }

            --sb.Length;

            CreateFile(savepath_data);
            SaveFile(savepath_data, sb.ToString());
        }

        void CreateFile(string path)
        {
            // 없으면 파일 생성
            if (!System.IO.File.Exists(path))
            {
                var stream = System.IO.File.Create(path);
                stream.Close();
            }

        }
        void SaveFile(string path, string text)
        {
            System.IO.File.WriteAllText(path, text);
        }
    }
}