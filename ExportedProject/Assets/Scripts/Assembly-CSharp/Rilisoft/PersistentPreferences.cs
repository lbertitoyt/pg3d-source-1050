using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PersistentPreferences : Preferences
	{
		private const string KeyElement = "Key";

		private const string PreferenceElement = "Preference";

		private const string RootElement = "Preferences";

		private const string ValueElement = "Value";

		private readonly XDocument _doc;

		private static readonly string _path;

		public override ICollection<string> Keys
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override ICollection<string> Values
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override int Count
		{
			get
			{
				return ((XContainer)_doc.Root).Elements().Count();
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal static string Path
		{
			get
			{
				return _path;
			}
		}

		public PersistentPreferences()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			try
			{
				_doc = XDocument.Load(_path);
			}
			catch (Exception)
			{
				_doc = new XDocument(new object[1] { (object)new XElement("Preferences") });
				_doc.Save(_path);
			}
		}

		static PersistentPreferences()
		{
			_path = System.IO.Path.Combine(Application.persistentDataPath, "com.P3D.Pixlgun.Settings.xml");
		}

		protected override void AddCore(string key, string value)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			XElement val = ((XContainer)_doc.Root).Elements("Preference").FirstOrDefault((XElement e) => ((XContainer)e).Element("Key") != null && ((XContainer)e).Element("Key").Value.Equals(key));
			if (val != null)
			{
				((XNode)val).Remove();
			}
			XElement val2 = new XElement("Preference", new object[2]
			{
				(object)new XElement("Key", (object)key),
				(object)new XElement("Value", (object)value)
			});
			((XContainer)_doc.Root).Add((object)val2);
			_doc.Save(_path);
		}

		protected override bool ContainsKeyCore(string key)
		{
			return ((XContainer)_doc.Root).Elements("Preference").Any((XElement e) => ((XContainer)e).Element("Key") != null && ((XContainer)e).Element("Key").Value.Equals(key));
		}

		protected override void CopyToCore(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		protected override bool RemoveCore(string key)
		{
			XElement val = ((XContainer)_doc.Root).Elements("Preference").FirstOrDefault((XElement e) => ((XContainer)e).Element("Key") != null && ((XContainer)e).Element("Key").Value.Equals(key));
			if (val != null)
			{
				((XNode)val).Remove();
				_doc.Save(_path);
				return true;
			}
			return false;
		}

		protected override bool TryGetValueCore(string key, out string value)
		{
			XElement val = ((XContainer)_doc.Root).Elements("Preference").FirstOrDefault((XElement e) => ((XContainer)e).Element("Key") != null && ((XContainer)e).Element("Key").Value.Equals(key));
			if (val != null)
			{
				XElement val2 = ((XContainer)val).Element("Value");
				if (val2 != null)
				{
					value = val2.Value;
					return true;
				}
			}
			value = null;
			return false;
		}

		public override void Save()
		{
			_doc.Save(_path);
		}

		public override void Clear()
		{
			((XContainer)_doc.Root).RemoveNodes();
			_doc.Save(_path);
		}

		public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			throw new NotSupportedException();
		}
	}
}
