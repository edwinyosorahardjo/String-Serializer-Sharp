﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringSerializer
{
    public class DelimitedString : BaseObjectString<DelimitedField>
    {
        private bool hasEnclosure = false;

        public override DelimitedField[] Fields { get; set; }
        public char Separator { get; set; }
        public char Enclosure { get; set; }

        #region constructors
        public DelimitedString(char fieldSeparator)
        {
            this.Separator = fieldSeparator;
        }

        public DelimitedString(char fieldSeparator, char fieldEnclosure)
            : this(fieldSeparator)
        {
            this.Enclosure = fieldEnclosure;
            hasEnclosure = true;
        }
        #endregion

        public T Deserialize<T>(string objectString) where T : new()
        {
            string pattern = hasEnclosure ? String.Format(Constants.DelimitedExpressions.WITHENCLOSURES, Separator, Enclosure)
                : String.Format(Constants.DelimitedExpressions.WITHOUTENCLOSURES, Separator);
            string[] stringFields = Regex.Split(objectString, String.Format(pattern, "\""));
            for (int i = 0; i < stringFields.Length; i++) stringFields[i] = stringFields[i].Trim(Enclosure);

            return base.Deserialize<T>(stringFields);
        }

        public string Serialize(object obj)
        {
            Type type = obj.GetType();

            StringBuilder sb = new StringBuilder();

            var orderedFields = Fields.OrderBy(a => a.Position);

            int cnt = 0, fieldCount = orderedFields.Count();
            foreach (var field in orderedFields)
            {
                cnt++;

                string fieldValue = type.GetProperty(field.PropertyName).GetValue(obj, null).ToString();
                sb.Append((hasEnclosure ? Enclosure + fieldValue + Enclosure : fieldValue));

                if (cnt != fieldCount) sb.Append(Separator);
            }

            return sb.ToString();
        }
    }
}
