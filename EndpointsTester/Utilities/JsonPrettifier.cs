using System.Text.Json.Nodes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace EndpointsTester.Utilities
{
    internal static class JsonPrettifier
    {
        public static Brush KeyColor { get; set; } = Brushes.DodgerBlue;
        public static Brush StringColor { get; set; } = Brushes.Orange;
        public static Brush NumberColor { get; set; } = Brushes.LightGreen;
        public static Brush BoolColor { get; set; } = Brushes.MediumPurple;
        public static Brush NullColor { get; set; } = Brushes.Gray;
        public static Brush SyntaxColor { get; set; } = Brushes.Gray;

        public static RichTextBox Format(string json)
        {
            var rtb = new RichTextBox();

            var doc = new FlowDocument();
            var p = new Paragraph();
            doc.Blocks.Add(p);

            try
            {
                var node = JsonNode.Parse(json);
                Write(node, p, 0);
            }
            catch
            {
                p.Inlines.Add(new Run(json));
            }

            rtb.Document = doc;
            return rtb;
        }

        private static void Write(JsonNode? node, Paragraph p, int indent)
        {
            var pad = new string(' ', indent * 2);

            switch (node)
            {
                case JsonObject obj:
                    {
                        p.Inlines.Add(new Run(pad + "{\n") { Foreground = SyntaxColor });

                        int count = obj.Count;
                        int i = 0;

                        foreach (var (key, value) in obj)
                        {
                            i++;

                            p.Inlines.Add(new Run(pad + "  \"") { Foreground = SyntaxColor });
                            p.Inlines.Add(new Run(key) { Foreground = KeyColor });
                            p.Inlines.Add(new Run("\": "));

                            WriteValue(value, p, indent + 1);

                            p.Inlines.Add(new Run(i < count ? ",\n" : "\n"));
                        }

                        p.Inlines.Add(new Run(pad + "}") { Foreground = SyntaxColor });
                        break;
                    }

                case JsonArray arr:
                    {
                        p.Inlines.Add(new Run("[\n") { Foreground = SyntaxColor });

                        int count = arr.Count;
                        int i = 0;

                        foreach (var item in arr)
                        {
                            i++;

                            WriteValue(item, p, indent + 1);

                            p.Inlines.Add(new Run(i < count ? ",\n" : "\n"));
                        }

                        p.Inlines.Add(new Run(pad + "]") { Foreground = SyntaxColor });
                        break;
                    }
            }
        }

        private static void WriteValue(JsonNode? node, Paragraph p, int indent)
        {
            if (node is null)
            {
                p.Inlines.Add(new Run("null") { Foreground = NullColor });
                return;
            }

            if (node is JsonObject || node is JsonArray)
            {
                Write(node, p, indent);
                return;
            }

            var text = node.ToString();

            if (node is JsonValue v)
            {
                if (v.TryGetValue<bool>(out _))
                {
                    p.Inlines.Add(new Run(text) { Foreground = BoolColor });
                    return;
                }

                if (v.TryGetValue<double>(out _))
                {
                    p.Inlines.Add(new Run(text) { Foreground = NumberColor });
                    return;
                }
            }

            p.Inlines.Add(new Run($"\"{text}\"") { Foreground = StringColor });
        }
    }
}