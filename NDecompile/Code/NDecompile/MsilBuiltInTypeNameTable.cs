using System;
using System.Collections.Generic;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile
{
    public static class MsilBuiltInTypeNameTable
    {
        private static Dictionary<String, String> TypeNameMappingTable = InitTypeNameMappingTable();

        private static Dictionary<String, String> InitTypeNameMappingTable()
        {
            Dictionary<String, String> table = new Dictionary<String, String>();

            table.Add("Void", "void");
            table.Add("Boolean", "bool");
            table.Add("Byte", "byte");
            table.Add("SByte", "sbyte");
            table.Add("Char", "char");
            table.Add("Int8", "int8");
            table.Add("Int16", "int16");
            table.Add("UInt16", "uint16");
            table.Add("Int32", "int32");
            table.Add("UInt32", "uint32");
            table.Add("Int64", "int64");
            table.Add("UInt64", "uint64");
            table.Add("String", "string");
            table.Add("Decimal", "decimal");
            table.Add("Double", "double");
            table.Add("Single", "float");

            table.Add("Boolean[]", "bool[]");
            table.Add("Byte[]", "byte[]");
            table.Add("SByte[]", "sbyte[]");
            table.Add("Char[]", "char[]");
            table.Add("Int8[]", "byte[]");
            table.Add("Int16[]", "int16[]");
            table.Add("UInt16[]", "uint16[]");
            table.Add("Int32[]", "int32[]");
            table.Add("UInt32[]", "uint32[]");
            table.Add("Int64[]", "int64[]");
            table.Add("UInt64[]", "uint64[]");
            table.Add("String[]", "string[]");
            table.Add("Decimal[]", "decimal[]");
            table.Add("Double[]", "double[]");
            table.Add("Single[]", "float[]");

            return table;
        }

        public static String Lookup(ITypeReference typeReference)
        {
            String name;
            TryLookup(typeReference, out name);
            return name;
        }

        public static bool TryLookup(ITypeReference typeReference, out String name)
        {
            if (typeReference.Namespace == "System")
            {
                if (TypeNameMappingTable.TryGetValue(typeReference.Name, out name))
                    return true;
            }

            name = typeReference.Name;

            return false;
        }

    }
}
