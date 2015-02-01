﻿namespace Il2Native.Logic.Gencode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using CodeParts;

    using Il2Native.Logic.Gencode.SynthesizedMethods;

    using PEAssemblyReader;

    public class ArrayMultiDimensionGen
    {
        /*
        public ArrayMultiDimensionGen_Ctor()
        {
            this.rank = 2;
            this.typeCode = type.GetElementType().TypeCode;
            this.elementSize = type.GetSize();
            this.length = dim1 * dim2;
            this.lowerBounds = new int[2] { 0, 0 };
            this.bounds = new int[2] { dim1, dim2 };
        }
         */

        public static void GetMultiDimensionArrayCtor(
            IType type,
            ITypeResolver typeResolver,
            out object[] code,
            out IList<object> tokenResolutions,
            out IList<IType> locals,
            out IList<IParameter> parameters)
        {
            var codeList = new List<object>();

            var rank = BitConverter.GetBytes((int)type.ArrayRank);
            var typeCode = BitConverter.GetBytes((int)type.GetTypeCode());
            var elementSize = BitConverter.GetBytes((int)type.GetTypeSize(typeResolver, true));

            codeList.AddRange(
                new object[]
                    {
                        Code.Ldarg_0,
                        Code.Dup,
                        Code.Dup,
                        Code.Ldc_I4,
                        (byte)rank[0],
                        (byte)rank[1],
                        (byte)rank[2],
                        (byte)rank[3],
                        Code.Stfld,
                        1,
                        0,
                        0,
                        0,
                        Code.Ldc_I4,
                        (byte)typeCode[0],
                        (byte)typeCode[1],
                        (byte)typeCode[2],
                        (byte)typeCode[3],
                        Code.Stfld,
                        2,
                        0,
                        0,
                        0,
                        Code.Ldc_I4,
                        (byte)elementSize[0],
                        (byte)elementSize[1],
                        (byte)elementSize[2],
                        (byte)elementSize[3],
                        Code.Stfld,
                        3,
                        0,
                        0,
                        0,
                    });

            // init lowerBounds
            // set all 0
            codeList.AddRange(
                new object[]
                {
                        Code.Ldc_I4,
                        (byte)rank[0],
                        (byte)rank[1],
                        (byte)rank[2],
                        (byte)rank[3],
                        Code.Newarr,
                        4,
                        0,
                        0,
                        0,
                        Code.Stloc_0,
                });

            // init each item in lowerBounds
            foreach (var i in Enumerable.Range(0, type.ArrayRank))
            {
                var index = BitConverter.GetBytes((int)i);
                codeList.AddRange(
                    new object[]
                {
                        Code.Ldloc_0,
                        Code.Ldc_I4,
                        (byte)index[0],
                        (byte)index[1],
                        (byte)index[2],
                        (byte)index[3],
                        Code.Ldc_I4_0,
                        Code.Stelem_I4
                });
            }

            // save new array into field lowerBounds
            codeList.AddRange(
                new object[]
                {
                        Code.Ldarg_0,
                        Code.Ldloc_0,
                        Code.Stfld,
                        5,
                        0,
                        0,
                        0,
                });

            // init Bounds
            codeList.AddRange(
                new object[]
                {
                        Code.Ldc_I4,
                        (byte)rank[0],
                        (byte)rank[1],
                        (byte)rank[2],
                        (byte)rank[3],
                        Code.Newarr,
                        6,
                        0,
                        0,
                        0,
                        Code.Stloc_1,
                });

            // init each item in lowerBounds
            foreach (var i in Enumerable.Range(0, type.ArrayRank))
            {
                var index = BitConverter.GetBytes((int)i);
                codeList.AddRange(
                    new object[]
                    {
                        Code.Ldloc_1,
                        Code.Ldc_I4,
                        (byte)index[0],
                        (byte)index[1],
                        (byte)index[2],
                        (byte)index[3],
                    });

                switch (i)
                {
                    case 0:
                        codeList.Add(Code.Ldarg_1);
                        break;
                    case 1:
                        codeList.Add(Code.Ldarg_2);
                        break;
                    case 2:
                        codeList.Add(Code.Ldarg_3);
                        break;
                    default:
                        var argIndex = BitConverter.GetBytes((int)i + 1);
                        codeList.AddRange(
                            new object[]
                            {
                                Code.Ldarg,
                                (byte)argIndex[0],
                                (byte)argIndex[1],
                                (byte)argIndex[2],
                                (byte)argIndex[3],
                            });
                        break;
                }

                codeList.AddRange(
                    new object[]
                    {
                        Code.Stelem_I4
                    });
            }

            // save new array into field lowerBounds
            codeList.AddRange(
                new object[]
                {
                        Code.Ldarg_0,
                        Code.Ldloc_1,
                        Code.Stfld,
                        7,
                        0,
                        0,
                        0,
                });

            // return
            codeList.AddRange(
                new object[]
                {
                        Code.Ret
                });

            // locals
            locals = new List<IType>();
            locals.Add(typeResolver.ResolveType("System.Int32").ToArrayType(1));
            locals.Add(typeResolver.ResolveType("System.Int32").ToArrayType(1));

            // tokens
            tokenResolutions = new List<object>();
            tokenResolutions.Add(type.GetFieldByName("rank", typeResolver));
            tokenResolutions.Add(type.GetFieldByName("typeCode", typeResolver));
            tokenResolutions.Add(type.GetFieldByName("elementSize", typeResolver));
            // lowerBounds
            tokenResolutions.Add(typeResolver.ResolveType("System.Int32").ToArrayType(1));
            tokenResolutions.Add(type.GetFieldByName("lowerBounds", typeResolver));
            // bounds
            tokenResolutions.Add(typeResolver.ResolveType("System.Int32").ToArrayType(1));
            tokenResolutions.Add(type.GetFieldByName("lengths", typeResolver));

            // code
            code = codeList.ToArray();

            // parameters
            var intType = typeResolver.ResolveType("System.Int32");
            parameters = Enumerable.Range(0, type.ArrayRank).Select(n => intType.ToParameter()).ToList();
        }
    }
}
