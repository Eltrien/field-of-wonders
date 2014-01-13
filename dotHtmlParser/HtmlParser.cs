using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace dotHtmlParser
{
    public static class HtmlParser
    {
        public static HtmlAgilityPack.HtmlNode GetDocNode(String content)
        {
            HtmlDocument doc = new HtmlDocument();
            try
            {
                doc.LoadHtml(content);
            }
            catch { }

            return doc.DocumentNode;
        }
        public static String GetSiblingInnerText(String xpath, String content)
        {
            String result = String.Empty;
            try
            {
                var docNode = GetDocNode(content);
                HtmlNode node = docNode.SelectSingleNode(xpath);
                result = node.NextSibling.InnerText;
            }
            catch { 
            }
            return result;
        }
        public static String GetInnerHtml(String xpath, String content)
        {
            String result = String.Empty;
            try
            {
                var docNode = GetDocNode(content);
                HtmlNode node = docNode.SelectSingleNode(xpath);
                result = node.InnerHtml;
            }
            catch
            {
            }
            return result;
        }
        public static String GetInnerText(String xpath, String content)
        {
            String result = String.Empty;
            try
            {
                var docNode = GetDocNode(content);
                HtmlNode node = docNode.SelectSingleNode(xpath);
                result = node.InnerText;
            }
            catch
            {
            }
            return result;
        }
        public static List<KeyValuePair<String, String>> FormParams(String xpath, String content)
        {
            List<KeyValuePair<String, String>> result = new List<KeyValuePair<String, String>>();
            try
            {
                var docNode = GetDocNode(content);
                var inputNodes = docNode.SelectNodes(xpath + "//input");
                if (inputNodes != null)
                {
                    foreach (HtmlNode node in inputNodes)
                    {
                        var id = node.GetAttributeValue("id", "");
                        if (String.IsNullOrEmpty(id))
                            id = node.GetAttributeValue("name", "");

                        var value = node.GetAttributeValue("value", "");
                        result.Add(new KeyValuePair<String, String>(id, value));
                    }
                }
                var selectedOptions = docNode.SelectNodes(xpath + @"//select/option[@selected='selected']");
                if (selectedOptions != null)
                {
                    foreach (HtmlNode node in selectedOptions)
                    {
                        var id = node.GetAttributeValue("value", "");
                        var value = node.NextSibling.InnerText;
                        result.Add(new KeyValuePair<String, String>(id, value));
                    }
                }

                var textAreas = docNode.SelectNodes(xpath + "//textarea");
                if (textAreas != null)
                {
                    foreach (HtmlNode node in textAreas)
                    {
                        var id = node.GetAttributeValue("id", "");
                        if (String.IsNullOrEmpty(id))
                            id = node.GetAttributeValue("name", "");

                        var value = node.InnerText;
                        result.Add(new KeyValuePair<String, String>(id, value));
                    }
                }

            }
            catch { }
            return result;
        }
        public static List<KeyValuePair<String,String>> GetOptions(String xpath, String content)
        {
            List<KeyValuePair<String, String>> result = new List<KeyValuePair<String, String>>();
            try
            {
                var docNode = GetDocNode(content);
                foreach (HtmlNode node in docNode.SelectNodes(xpath))
                {
                    result.Add(new KeyValuePair<String, String>(node.GetAttributeValue("value", ""), node.NextSibling.InnerText));
                }
            }
            catch { }
            return result;
        }
        public static String GetAttribute(String xpath, String attribName, String content)
        {
            String result = String.Empty; 

            try
            {
                var docNode = GetDocNode(content);
                HtmlNode node = docNode.SelectSingleNode(xpath);
                result = node.Attributes[attribName].Value;
            }
            catch
            {
            }
            return result;
        }


    }
}
