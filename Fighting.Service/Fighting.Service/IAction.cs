using System;
using System.Collections;
namespace Fighting.Service
{
	internal interface IAction
	{
		string Name
		{
			get;
		}
		string Syntax
		{
			get;
		}
		string Description
		{
			get;
		}
		void OnAction(System.Collections.Hashtable parameters);
	}
}
