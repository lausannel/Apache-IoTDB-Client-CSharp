using System;
using System.Collections.Generic;
using System.Linq;
using Thrift;

namespace Apache.IoTDB.DataStructure
{
    public class RowRecord
    {
        public long Timestamps { get; }
        public List<object> Values { get; }
        public List<string> Measurements { get; }

        public RowRecord(long timestamps, List<object> values, List<string> measurements)
        {
            Timestamps = timestamps;
            Values = values;
            Measurements = measurements;
        }

        public void Append(string measurement, object value)
        {
            Values.Add(value);
            Measurements.Add(measurement);
        }

        public DateTime GetDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamps).DateTime.ToLocalTime();
        }

        public override string ToString()
        {
            var str = "TimeStamp";
            foreach (var measurement in Measurements)
            {
                str += "\t\t";
                str += measurement;
            }

            str += "\n";

            str += Timestamps.ToString();
            foreach (var rowValue in Values)
            {
                str += "\t\t";
                str += rowValue.ToString();
            }

            return str;
        }

        public List<int> GetDataTypes()
        {
            var dataTypeValues = new List<int>();
            
            foreach (var valueType in Values.Select(value => value))
            {
                switch (valueType)
                {
                    case bool _:
                        dataTypeValues.Add((int) TSDataType.BOOLEAN);
                        break;
                    case int _:
                        dataTypeValues.Add((int) TSDataType.INT32);
                        break;
                    case long _:
                        dataTypeValues.Add((int) TSDataType.INT64);
                        break;
                    case float _:
                        dataTypeValues.Add((int) TSDataType.FLOAT);
                        break;
                    case double _:
                        dataTypeValues.Add((int) TSDataType.DOUBLE);
                        break;
                    case string _:
                        dataTypeValues.Add((int) TSDataType.TEXT);
                        break;
                }
            }

            return dataTypeValues;
        }

        public byte[] ToBytes()
        {
            var buffer = new ByteBuffer(Values.Count * 8);
            
            foreach (var value in Values)
            {
                switch (value)
                {
                    case bool b:
                        buffer.AddByte((byte) TSDataType.BOOLEAN);
                        buffer.AddBool(b);
                        break;
                    case int i:
                        buffer.AddByte((byte) TSDataType.INT32);
                        buffer.AddInt(i);
                        break;
                    case long l:
                        buffer.AddByte((byte) TSDataType.INT64);
                        buffer.AddLong(l);
                        break;
                    case double d:
                        buffer.AddByte((byte) TSDataType.DOUBLE);
                        buffer.AddDouble(d);
                        break;
                    case float f:
                        buffer.AddByte((byte) TSDataType.FLOAT);
                        buffer.AddFloat(f);
                        break;
                    case string s:
                        buffer.AddByte((byte) TSDataType.TEXT);
                        buffer.AddStr(s);
                        break;
                    default:
                        throw new TException($"Unsupported data type:{value.GetType()}", null);
                }
            }

            return buffer.GetBuffer();;
        }
    }
}