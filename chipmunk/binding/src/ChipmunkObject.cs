//
// ChipmunkObject.cs
//
// Authors:
//  Stephane Delcroix <stephane@delcroix.org>
//
// Copyright 2012 S. Delcroix
//

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Chipmunk
{
    public class ChipmunkObject : IDisposable
    {
	static Dictionary<IntPtr, List<ChipmunkObject>> references = new Dictionary<IntPtr, List<ChipmunkObject>> ();

	public HandleRef Handle { get; private set;}

	public void Dispose ()
	{
	    Cleanup ();

	    System.GC.SuppressFinalize (this);
	}

	protected ChipmunkObject (IntPtr ptr)
	{
	    Handle = new HandleRef (this, ptr);
	    AddRef (this, ptr);
	}
	
	~ChipmunkObject ()
	{
	    Cleanup ();
	}

	void Cleanup ()
	{
	    if (RemoveRef (this, Handle.Handle))
		Free ();
	    Handle = new HandleRef (this, IntPtr.Zero);
	}

	protected virtual void Free ()
	{
	}

	static void AddRef (ChipmunkObject obj, IntPtr ptr)
	{
	    List<ChipmunkObject> list;
	    if (!references.TryGetValue (ptr, out list)){
		list = new List<ChipmunkObject> ();
		references.Add (ptr, list);
	    }
	    list.Add (obj);
	}

	static bool RemoveRef (ChipmunkObject obj, IntPtr ptr)
	{
	    List<ChipmunkObject> list;
	    if (references.TryGetValue (ptr, out list)) {
		list.Remove (obj);
		if (list.Count == 0) {
		    references.Remove (ptr);
		    return true;
		}
		return false;
	    }
#if DEBUG
	    throw new InvalidOperationException ();
#endif
	    return false;
	}
    }    
}
