using System;
using System.Xml;

namespace Expression_Builder_Dev_Helper {
  class MainClass {
    // Enter Expression Builder code in this function
    public static string ExpressionBuilder() {
      // ============================================================================================================================
      // ============================================================================================================================
      // ==================================== YOUR EXPRESSION BUILDER CODE GOES BELOW THIS LINE =====================================
      // ============================================================================================================================
      // ============================================================================================================================
      var trackedContents = _context.XmlVariables["contractTrackedValue"].GetXmlNode("/xml");
      var ret = new XmlDocument();
      ret.LoadXml("<root />");

      Func<string, string, XmlDocument, XmlNode> mkNode = (tag, innerText, targetDocument) => {
        var ret = targetDocument.CreateElement(tag);
        ret.InnerText = innerText;
        return ret;
      };

      Func<XmlDocument, XmlNode> getBr = (targetDocument) => {
        var br = targetDocument.CreateElement("br");
        return br;
      };

      Func<XmlDocument, Func<string, string, XmlNode>> mkNode2 = (targetDocument) => {
        return (tag, innerText) => mkNode(tag, innerText, targetDocument);
      };

      Func<XmlNode, XmlDocument, XmlNamespaceManager, XmlNode> wpToHtml = (wp, targetDocument, nsmgr) => {
        var mkNode = mkNode2(targetDocument);
        var wr = wp.SelectSingleNode("w:r", nsmgr);
        if (wr == null) {
          return mkNode("p", "");
        }
        if (wr.SelectSingleNode("w:rPr/w:u", nsmgr) != null) {
          return mkNode("p", "").AppendChild(mkNode("u", wr.InnerText));
        } else if (wr.SelectSingleNode("w:rPr/w:b", nsmgr) != null) {
          return mkNode("p", "").AppendChild(mkNode("b", wr.InnerText));
        } else {
          return mkNode("p", wr.InnerText);
        }
      };

      foreach (XmlNode trackedContent in trackedContents) {
        var mkNodee = mkNode2(ret);
        var val = trackedContent.SelectSingleNode("Value").InnerText;
        var wpml = new XmlDocument();
        wpml.LoadXml("<root>" + val + "</root>");
        var nsmgr = new XmlNamespaceManager(wpml.NameTable);
        nsmgr.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        var nodes = wpml.SelectNodes("/root/w:p", nsmgr);
        var trackedContentRoot = mkNodee("div", "");
        ret.SelectSingleNode("/root").AppendChild(trackedContentRoot);
        foreach (XmlNode wp in nodes) {
          if (wp.SelectSingleNode("w:r", nsmgr) != null) {
            trackedContentRoot.AppendChild(wpToHtml(wp, ret, nsmgr));
            trackedContentRoot.AppendChild(getBr(ret));
          }
        }
      }

      return ret.OuterXml;
      // ============================================================================================================================
      // ============================================================================================================================
      // ==================================== YOUR EXPRESSION BUILDER CODE GOES ABOVE THIS LINE =====================================
      // ============================================================================================================================
      // ============================================================================================================================
    }

    static Func<string, string> Base64Encode = plainText => System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));

    static Func<string, string> Base64Decode = base64EncodedData => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64EncodedData));

    public static void prefill_context() {
      string[] files = System.IO.Directory.GetFiles(@"xml/", "*");
      foreach (string file in files) {
        var filename = file.Substring(4);
        var key = filename.Remove(filename.Length - 4, 4);
        _context.XmlVariables.Add(key, new context_content(filename));
      }
    }

    // Here you can define your workflow variable contents
    public static string GetVariableValue(string varName) {
      if (varName == "^Choice 1Comments") { return Base64Decode("RG9jdW1lbnQgcmV2aWV3IGZvciBhIERhdGEgQ2hhbmdlIGZvciBCb1J1IFNhbWFyb28gKFNFU0E0NDIzNDkpIFJlamVjdCBhbmQgU2VuZCBCYWNrIHRvIEhSQlAgYnkgRGF3aWQgQm90aGEgKG9uIGJlaGFsZiBvZiBIUlNfU1dFKSBvbiA1LzE0LzIwMjAgYXQgMzowOTo1NiBQTSB3aXRoIGNvbW1lbnQ6IA0KNw0KHkRvY3VtZW50IHJldmlldyBmb3IgYSBEYXRhIENoYW5nZSBmb3IgQm9SdSBTYW1hcm9vIChTRVNBNDQyMzQ5KSBSZWplY3QgYW5kIFNlbmQgQmFjayB0byBIUkJQIGJ5IERhd2lkIEJvdGhhIChvbiBiZWhhbGYgb2YgSFJTX1NXRSkgb24gNS8xNC8yMDIwIGF0IDM6MTE6NDUgUE0gd2l0aCBjb21tZW50OiANCjkNCg=="); }
      if (varName == "startTime") { return "1595336388"; }
      throw new Exception("No matching variable value found for " + varName);
    }

    static context _context = new context();
    public static void Main(string[] args) {
      prefill_context();
      var expressionBuilderOutput = ExpressionBuilder();

      //Test XML validity. Comment out if not applicable.
      // XmlDocument doc = new XmlDocument();
      // doc.LoadXml(expressionBuilderOutput);

      Console.WriteLine(expressionBuilderOutput);
    }
  }
  class context {
    public System.Collections.Generic.Dictionary<string, context_content> XmlVariables = new System.Collections.Generic.Dictionary<string, context_content>();
  }
  class context_content {
    XmlDocument doc = new XmlDocument();
    public context_content(string filename) {
      doc.Load("xml/" + filename);
    }
    public XmlNode GetXmlNode(string xpath) {
      return doc.SelectSingleNode(xpath);
    }
  }
}
