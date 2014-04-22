using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.ScriptClasses
{
    public class ScriptInterpreter
    {
        #region properties
        public List<string> Values = new List<string>();
        public string Property;

        private List<string> lines = new List<string>();
        private int activeLine = 0, skipBrace = 0, condBrace = 0, parenOpen = 0, condParan = 0;
        private bool quoteActive = false, noConditions = true, startReading = false;
        private StringBuilder valueSB = new StringBuilder(), wrapper = new StringBuilder();
        private string condstring = null;
        #endregion

        #region constructor
        private static ScriptInterpreter instance;
        private ScriptInterpreter() { }

        public static ScriptInterpreter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScriptInterpreter();
                }
                return instance;
            }
        }
        #endregion

        #region main script functions
        public void loadFile(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string replacement = Regex.Replace(line, @"\t|\r", "");
                    lines.Add(replacement); // Add to list.
                }
            }
        }

        private void setValue(string value)
        {
            Values.Add(value.Trim()); // trim is remove spaces
        }

        public bool StartReading 
        { 
            get { return startReading; }
            set { startReading = value; }
        }

        private void clearStringBuilders()
        {
            valueSB.Length = 0;
            valueSB.Capacity = 0;
            valueSB.Clear();
            wrapper.Length = 0;
            wrapper.Capacity = 0;
            wrapper.Clear();
        }

        private void clearValues()
        {
            Values.Clear();
        }

        public void clearInstance()
        {
            this.Values.Clear();
            this.lines.Clear();
            this.clearStringBuilders();
            this.noConditions = true;
            this.condstring = null;
            this.clearValues();
            this.Property = null;
            this.startReading = false;

            activeLine = 0;
            skipBrace = 0;
            condBrace = 0;
            parenOpen = 0;
            condParan = 0;
        }

        public void setChooise(int selectedIndex)
        {
            this.condstring = "case " + selectedIndex + ":";
            noConditions = false;
            clearValues();
        }
        #endregion

        public void readScript()
        {
            Property = null;

            while (Property == null)
            {
                if (noConditions)
                {
                    for (int i = 0; i < lines[activeLine].Length; i++)
                    {
                        #region getchar and stringbuilder
                        wrapper.Append(lines[activeLine][i]);

                        string getchar = wrapper[wrapper.Length - 1].ToString(),
                               previouschar = "", nextchar = "";

                        if (wrapper.Length - 2 >= 0)
                            previouschar = wrapper[wrapper.Length - 2].ToString();

                        if (i + 1 < lines[activeLine].Length)
                            nextchar = lines[activeLine][i + 1].ToString();

                        switch (getchar)
                        {
                            case "{":
                                skipBrace++;
                                break;
                            case "}":
                                skipBrace--;
                                break;
                            case "(":
                                parenOpen++;
                                break;
                            case ")":
                                parenOpen--;
                                break;
                            case "\"":
                                quoteActive = !quoteActive; 	// flip boolean
                                break;
                            case ";":
                                if (valueSB.ToString() != "" && !quoteActive)
                                {
                                    setValue(valueSB.ToString());
                                    valueSB.Length = 0;
                                    valueSB.Capacity = 0;
                                    valueSB.Clear();
                                }
                                break;
                        }
                        #endregion
                        #region NPC finder
                        // Find the right NPC
                        if (!startReading)
                        {
                            if (wrapper.ToString().StartsWith("npc"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "npc";
                                    this.clearValues();
                                }
                                else
                                {
                                    switch (getchar)
                                    {
                                        case " ":
                                            if (!quoteActive)
                                                valueSB.Clear();
                                            else
                                                valueSB.Append(lines[activeLine][i]);
                                            break;
                                        case "\"":
                                            if (!quoteActive)
                                            {
                                                setValue(valueSB.ToString());
                                                valueSB.Clear();
                                            }
                                            break;
                                        default:
                                            valueSB.Append(lines[activeLine][i]);
                                            break;
                                    }
                                }

                            }
                        }
                        else
                        {
                        #endregion
                            #region default commands
                            // COMMANDS
                            if (wrapper.ToString().StartsWith("next"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "next";
                                    clearValues();
                                }
                            }
                            else if (wrapper.ToString().StartsWith("close"))
                            {
                                if (this.Property == null)
                                    this.Property = "close";
                            }
                            else if (wrapper.ToString().StartsWith("mes"))
                            {
                                if (this.Property == null)
                                    this.Property = "mes";

                                if (quoteActive && getchar != "\"")
                                    valueSB.Append(lines[activeLine][i]);
                            }
                            #endregion
                            #region special commands
                            // SPECIAL COMMANDS
                            else if (wrapper.ToString().StartsWith("getitem"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "getitem";
                                    this.clearValues();
                                }
                                switch (getchar)
                                {
                                    case " ":
                                        if (valueSB.Length > 1)
                                            setValue(valueSB.ToString());
                                        valueSB.Clear();
                                        break;
                                    case ";":
                                        setValue(valueSB.ToString());
                                        valueSB.Clear();

                                        // value 0 = itemID, value 1= itemCount
                                        if (Values.Count > 0)
                                        {
                                            for (int getItem = 0; getItem < Convert.ToInt32(Values[1]); getItem++)
                                            {
                                                ItemClasses.Inventory.Instance.addItem(
                                                        ItemClasses.ItemStore.Instance.getItem(Convert.ToInt32(Values[0]))
                                                    );
                                            }
                                        }
                                        Values.Clear();
                                        break;
                                    default:
                                        valueSB.Append(lines[activeLine][i]);
                                        break;
                                }
                            }
                            else if (wrapper.ToString().StartsWith("delitem"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "delitem";
                                    this.clearValues();
                                }
                                switch (getchar)
                                {
                                    case " ":
                                        if (valueSB.Length > 1)
                                            setValue(valueSB.ToString());
                                        valueSB.Clear();
                                        break;
                                    case ";":
                                        setValue(valueSB.ToString());
                                        valueSB.Clear();

                                        // value 0 = itemID, value 1 = itemCount
                                        if (Values.Count > 0)
                                        {
                                            for (int getItem = 0; getItem < Convert.ToInt32(Values[1]); getItem++)
                                            {
                                                ItemClasses.Inventory.Instance.removeItem(
                                                        Convert.ToInt32(Values[0])
                                                    );
                                            }
                                        }
                                        Values.Clear();
                                        break;
                                    default:
                                        valueSB.Append(lines[activeLine][i]);
                                        break;
                                }
                            }
                            else if (wrapper.ToString().StartsWith("setswitch"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "setswitch";
                                    this.clearValues();
                                }
                                switch (getchar)
                                {
                                    case " ":
                                        if (valueSB.Length > 1)
                                            setValue(valueSB.ToString());
                                        valueSB.Clear();
                                        break;
                                    case ";":
                                        setValue(valueSB.ToString());
                                        valueSB.Clear();

                                        // value 0 = switchName (string), value 1= switchValue (string)
                                        if (Values.Count > 0)
                                        {
                                            SwitchStore.Instance.setSwitch(
                                                    SwitchStore.Instance.switch_list.Count,
                                                    Values[0].ToString(),
                                                    Values[1].ToString());
                                        }
                                        Values.Clear();
                                        break;
                                    default:
                                        valueSB.Append(lines[activeLine][i]);
                                        break;
                                }
                            }
                            #endregion
                            #region chooise / case statement
                            // PLAYER MENU CHOOISE
                            else if (wrapper.ToString().StartsWith("chooise"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "chooise";
                                    this.clearValues();
                                    this.condParan = parenOpen;
                                }

                                if (parenOpen > condParan || (parenOpen == condParan && getchar == ")"))
                                {
                                    switch (getchar)
                                    {
                                        case ",":
                                        case ")":
                                            setValue(valueSB.ToString());
                                            valueSB.Clear();
                                            break;
                                        case "(":
                                            break;
                                        default:
                                            valueSB.Append(lines[activeLine][i]);
                                            break;
                                    }
                                }
                            }
                            else if (wrapper.ToString().StartsWith("openshop"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "openshop";
                                    this.clearValues();
                                    this.condParan = parenOpen;
                                }

                                if (parenOpen > condParan || (parenOpen == condParan && getchar == ")"))
                                {
                                    switch (getchar)
                                    {
                                        case ",":
                                        case ")":
                                            setValue(valueSB.ToString());
                                            valueSB.Clear();
                                            break;
                                        case "(":
                                            break;
                                        default:
                                            valueSB.Append(lines[activeLine][i]);
                                            break;
                                    }
                                }
                            }
                            #endregion
                            #region operators if statement
                            // OPERATORS
                            else if (wrapper.ToString().StartsWith("if"))
                            {
                                if (this.Property == null)
                                {
                                    this.Property = "if";
                                    this.clearValues();
                                    this.condParan = parenOpen;
                                }

                                if (parenOpen > condParan || (parenOpen == condParan && getchar == ")"))
                                {
                                    switch (getchar)
                                    {
                                        case " ":
                                            if (valueSB.Length > 1)
                                                setValue(valueSB.ToString());
                                            valueSB.Clear();
                                            break;
                                        case ")":
                                            setValue(valueSB.ToString());
                                            valueSB.Clear();
                                            if (!readCondition())
                                            {
                                                noConditions = false;
                                                skipBrace = 0;
                                                condBrace = 0;
                                            }

                                            Values.Clear();
                                            break;
                                        case "&":
                                            if (previouschar == "&")
                                            {
                                                setValue("AND");
                                                valueSB.Clear();
                                            }
                                            else
                                            {
                                                setValue(valueSB.ToString());
                                                valueSB.Clear();
                                            }
                                            break;
                                        case "|":
                                            if (previouschar == "|")
                                            {
                                                setValue("OR");
                                                valueSB.Clear();
                                            }
                                            else
                                            {
                                                setValue(valueSB.ToString());
                                                valueSB.Clear();
                                            }
                                            break;
                                        case "=":
                                            switch (previouschar)
                                            {
                                                case " ":
                                                    valueSB.Append(lines[activeLine][i]);
                                                    break;
                                                case "=":
                                                    valueSB.Length -= 1;                // remove previous '='
                                                    if (valueSB.Length > 1)
                                                        setValue(valueSB.ToString());

                                                    setValue("EQ");
                                                    valueSB.Clear();
                                                    break;
                                                case "<":
                                                    valueSB.Length -= 1;                // remove previous '='
                                                    if (valueSB.Length > 1)
                                                        setValue(valueSB.ToString());

                                                    setValue("SEQ");
                                                    valueSB.Clear();
                                                    break;
                                                case ">":
                                                    valueSB.Length -= 1;                // remove previous '='
                                                    if (valueSB.Length > 1)
                                                        setValue(valueSB.ToString());

                                                    setValue("BEQ");
                                                    valueSB.Clear();
                                                    break;
                                                default:
                                                    if (valueSB.Length > 1)
                                                        setValue(valueSB.ToString());
                                                    valueSB.Clear();
                                                    break;
                                            }
                                            break;
                                        case "(":
                                            break;
                                        default:
                                            valueSB.Append(lines[activeLine][i]);
                                            break;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    //Read the current line and count Braces
                    for (int i = 0; i < lines[activeLine].Length; i++)
                    {
                        wrapper.Append(lines[activeLine][i]);
                        string getchar = wrapper[wrapper.Length - 1].ToString();

                        if (getchar == "{")
                            skipBrace++; 
                        if (getchar == "}")
                            skipBrace--;
                    }

                    // condition cases without label
                    if (condstring == null)
                    {
                        //condition brace like { }
                        if (condBrace == skipBrace)
                        	 noConditions = true;
                    }
                    // condition case like "case 1: or case 2:"
                    else if (wrapper.ToString().StartsWith(condstring))
                    {
                        noConditions = true;
                    }
                }

                activeLine++; // line completed goto next line
                clearStringBuilders();
            }
        }

        private bool readCondition()
        {
            List<string> strValues = new List<string>();
            List<int> intValues = new List<int>();

            for (int index = 0; index < Values.Count; index++ )
            {
                if (index == Values.Count - 1 ||                    // avoid index out of bound
                    (Values[index + 1].ToString() != "AND" &&       // make sure we read the first value
                     Values[index + 1].ToString() != "OR")
                   )
                {
                    switch (Values[index].ToString())
                    {
                        case "baselevel":
                            intValues.Add(PlayerStore.Instance.activePlayer.Level);
                            continue;
                        case "gold":
                            intValues.Add(PlayerStore.Instance.activePlayer.Gold);
                            continue;
                        case "experience":
                            intValues.Add(PlayerStore.Instance.activePlayer.Exp);
                            continue;
                        case "item":
                                intValues.Add(ItemClasses.Inventory.Instance.item_list.FindAll(delegate(ItemClasses.Item item) 
                                {
                                    return item.itemID == Convert.ToInt32(Values[index + 1]); 
                                }).Count);
                                index++;
                                continue;
                        case "switch":
                                // does the switch exist
                                if (SwitchStore.Instance.switch_list.FindAll(delegate(Switch switchobj)
                                        { return switchobj.switchName == Values[index + 1]; }
                                    ).Count >= 1)
                                {
                                    strValues.Add(SwitchStore.Instance.switch_list.Find(delegate(Switch switchobj)
                                    {
                                        return switchobj.switchName == Values[index + 1];
                                    }).switchValue);
                                    index++;
                                }
                                else
                                {
                                    strValues.Add("null");
                                    index++;
                                }
                                continue;
                        case "profession":
                                strValues.Add(PlayerClasses.PlayerStore.Instance.activePlayer.Jobclass);
                            continue;
                        #region operators
                        case "EQ":
                            if (strValues.Count > 0)
                            {
                                if (strValues[0] == Values[index + 1].ToString())                       // String equal as TRUE
                                {
                                    if (Values.FindAll(delegate(string var) { 
                                        return var.ToString() == "AND"; }).Count > 0)
                                    {
                                        strValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no AND
                                        return true;
                                }
                                else                                                                    // Statement FALSE
                                {
                                    if (Values.FindAll(delegate(string var) { 
                                        return var.ToString() == "OR"; }).Count > 0)
                                    {
                                        strValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no OR
                                        return false;
                                }
                            }
                            else if (intValues.Count > 0)
                            {
                                if (intValues[0] == Convert.ToInt32(Values[index + 1]))                 // Equal as TRUE
                                {
                                    if (Values.FindAll(delegate(string var)
                                    {
                                        return var.ToString() == "AND";
                                    }).Count > 0)
                                    {
                                        intValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no AND
                                        return true;
                                }
                                else                                                                    // Statement FALSE
                                {
                                    if (Values.FindAll(delegate(string var)
                                    {
                                        return var.ToString() == "OR";
                                    }).Count > 0)
                                    {
                                        intValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no OR
                                        return false;
                                }
                            }
                            break;
                        case "SEQ":
                            if (intValues.Count > 0)
                                if (intValues[0] <= Convert.ToInt32(Values[index + 1]))                 // Smaller than TRUE
                                {
                                    if (Values.FindAll(delegate(string var)
                                    {
                                        return var.ToString() == "AND";
                                    }).Count > 0)
                                    {
                                        intValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no AND
                                        return true;
                                }
                                else                                                                    // Statement FALSE
                                {
                                    if (Values.FindAll(delegate(string var)
                                    {
                                        return var.ToString() == "OR";
                                    }).Count > 0)
                                    {
                                        intValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no OR
                                        return false;
                                }
                            break;
                        case "BEQ":
                            if (intValues.Count > 0)
                                if (intValues[0] >= Convert.ToInt32(Values[index + 1]))                 // Bigger than TRUE
                                {
                                    if (Values.FindAll(delegate(string var)
                                    {
                                        return var.ToString() == "AND";
                                    }).Count > 0)
                                    {
                                        intValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no AND
                                        return true;
                                }
                                else                                                                    // statement FALSE
                                {
                                    if (Values.FindAll(delegate(string var)
                                    {
                                        return var.ToString() == "OR";
                                    }).Count > 0)
                                    {
                                        intValues.Clear();
                                        continue;
                                    }
                                    else                                                                // no OR
                                        return false;
                                }
                            break;
                        #endregion
                        default:
                            continue;
                    }
                }
            }
            return false;
        }

    }
}
