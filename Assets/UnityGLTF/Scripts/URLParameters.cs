/* * * * *
 * URLParameters.cs
 * ----------------
 *
 * This singleton script provides easy access to any URL components in a Web-build
 * Just use
 *
 *     URLParameters.Instance.RegisterOnDone(OnDone);
 *    
 * To register a callback which will be invoked from javascript. The callback receives a
 * reference to the singleton instance. The instance has several properties to hold all the
 * different parts of the URL. In addition it will split and parse the search and hash /
 * fragment value into key/value pairs stored in a dictionary (SearchParameters, HashParameters)
 *
 * The MIT License (MIT)
 *
 * Copyright (c) 2012-2017 Markus Göbel (Bunny83)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * * * * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class URLParameters : MonoBehaviour
{
	// set testíng data here for in-editor-use
	// href | hash | host | hostname | pathname | port | protocol | search
	public static string TestData = "|||||||";
	private static URLParameters m_Instance = null;

	public static URLParameters Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = (URLParameters) FindObjectOfType(typeof(URLParameters));
				if (m_Instance == null)
					m_Instance = (new GameObject("URLParameters")).AddComponent<URLParameters>();
				m_Instance.gameObject.name = "URLParameters";
				DontDestroyOnLoad(m_Instance.gameObject);
			}

			return m_Instance;
		}
	}

	private System.Action<URLParameters> m_OnDone = null;
	private System.Action<URLParameters> m_OnDoneOnce = null;

	private bool m_HaveInformation = false;
	private string m_RawData;
	private string m_Href;
	private string m_Hash;
	private string m_Host;
	private string m_Hostname;
	private string m_Pathname;
	private string m_Port;
	private string m_Protocol;
	private string m_Search;
	private Dictionary<string, string> m_SearchParams = new Dictionary<string, string>();
	private Dictionary<string, string> m_HashParams = new Dictionary<string, string>();
	public string url;

	public bool HaveInformation
	{
		get { return m_HaveInformation; }
	}

	public string RawData
	{
		get { return m_RawData; }
	}

	public string Href
	{
		get { return m_Href; }
	}

	public string Hash
	{
		get { return m_Hash; }
	}

	public string Host
	{
		get { return m_Host; }
	}

	public string Hostname
	{
		get { return m_Hostname; }
	}

	public string Pathname
	{
		get { return m_Pathname; }
	}

	public string Port
	{
		get { return m_Port; }
	}

	public string Protocol
	{
		get { return m_Protocol; }
	}

	public string Search
	{
		get { return m_Search; }
	}

	public IDictionary<string, string> SearchParameters
	{
		get { return m_SearchParams; }
	}

	public IDictionary<string, string> HashParameters
	{
		get { return m_HashParams; }
	}


	public void RegisterOnDone(System.Action<URLParameters> aCallback)
	{
		Debug.Log("********RegisterOnDone");
		m_OnDone += aCallback;
		if (HaveInformation)
			aCallback(this);
	}


	public void RegisterOnceOnDone(System.Action<URLParameters> aCallback)
	{
		if (HaveInformation)
			aCallback(this);
		else
			m_OnDoneOnce += aCallback;
	}


	public void Request()
	{
		StartCoroutine(_Request());
	}

	private IEnumerator _Request()
	{
		m_HaveInformation = false;
#if UNITY_EDITOR
		yield return null;
		SetAddressComponents2(url);
#elif UNITY_WEBPLAYER
        const string WebplayerCode =
 "GetUnity ().SendMessage ('{0}', 'SetAddressComponents', location.href +'|'+ location.hash +'|'+ location.host +'|'+ location.hostname +'|'+ location.pathname +'|'+ location.port +'|'+ location.protocol +'|'+ location.search);";
        Application.ExternalEval(string.Format(WebplayerCode, gameObject.name));
#elif UNITY_WEBGL
        const string WebGLCode =
 "SendMessage ('{0}', 'SetAddressComponents', location.href +'|'+ location.hash +'|'+ location.host +'|'+ location.hostname +'|'+ location.pathname +'|'+ location.port +'|'+ location.protocol +'|'+ location.search);";
        Application.ExternalEval(string.Format(WebGLCode, gameObject.name));
#endif
		yield break;
	}

	public IEnumerator Start()
	{
		yield return null; // wait one frame to ensure all delegates are assigned.
		Request();
	}

	public string GetValue(string key)
	{
		if (SearchParameters == null)
		{
			Debug.LogError("没有字典");
			return "";
		}

		if (SearchParameters.ContainsKey(key))
		{
			return SearchParameters[key];
		}

		return "";
	}

	public void SetAddressComponents2(string aData)
	{
		//http://127.0.0.1:2000/examples/unity/index.html?model=Monster&scale=0.05

		var index = aData.IndexOf("examples");

		var root = aData.Substring(0, index);

		var q = aData.IndexOf("?") + 1;

		var pa = aData.Substring(q, aData.Length - q);

		var arr = pa.Split('&');

		foreach (string s in arr)
		{
			var ar2 = s.Split('=');
			m_SearchParams[ar2[0]] = ar2[1];
		}


		if (m_OnDone != null)
			m_OnDone(this);
		if (m_OnDoneOnce != null)
		{
			m_OnDoneOnce(this);
			m_OnDoneOnce = null;
		}
	}

	public void SetAddressComponents(string aData)
	{
		Debug.Log("********SetAddressComponents" + aData);
		string[] parts = aData.Split('|');
		m_RawData = aData;
		m_Href = parts[0];
		m_Hash = parts[1];
		m_Host = parts[2];
		m_Hostname = parts[3];
		m_Pathname = parts[4];
		m_Port = parts[5];
		m_Protocol = parts[6];
		m_Search = parts[7];
		var tmp = m_Search.TrimStart('?');
		var data = tmp.Split('&');
		Debug.Log("0********SetAddressComponents" + aData);
		for (int i = 0; i < data.Length; i++)
		{
			var val = data[i].Split('=');
			if (val.Length != 2)
				continue;

			m_SearchParams[val[0]] = val[1];
		}

		Debug.Log("2********SetAddressComponents" + aData);
		tmp = m_Hash.TrimStart('#');
		data = tmp.Split('&');
		for (int i = 0; i < data.Length; i++)
		{
			var val = data[i].Split('=');
			if (val.Length != 2)
				continue;
			m_HashParams[val[0]] = val[1];
		}

		Debug.Log("3********SetAddressComponents" + aData);
		m_HaveInformation = true;
		if (m_OnDone != null)
			m_OnDone(this);
		if (m_OnDoneOnce != null)
		{
			m_OnDoneOnce(this);
			m_OnDoneOnce = null;
		}
	}
}


public static class IDictionaryExtension
{
	public static double GetDouble(this IDictionary<string, string> aDict, string aKey, double aDefault = 0d)
	{
		string tmp;
		if (aDict.TryGetValue(aKey, out tmp))
		{
			double val;
			if (double.TryParse(tmp, out val))
				return val;
		}

		return aDefault;
	}

	public static int GetInt(this IDictionary<string, string> aDict, string aKey, int aDefault = 0)
	{
		string tmp;
		if (aDict.TryGetValue(aKey, out tmp))
		{
			int val;
			if (int.TryParse(tmp, out val))
				return val;
		}

		return aDefault;
	}
}
