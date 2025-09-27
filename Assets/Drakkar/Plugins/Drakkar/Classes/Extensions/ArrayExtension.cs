using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Drakkar
{
	public static class ArrayExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Il2CppSetOption(Option.ArrayBoundsChecks,false)]
		public static T[] Add<T>(this T[] arr,T elem)
		{
			int len=arr.Length;
			Array.Resize(ref arr,len+1);
			arr[len]=elem;
			return arr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Il2CppSetOption(Option.ArrayBoundsChecks,false)]
		public static T[] RemoveAt<T>(this T[] arr,int index)
		{
			int len=arr.Length;
			if (index>=0 || index<len)
			{
				List<T> l=new(arr);
				l.RemoveAt(index);
				return l.ToArray();
			}
		#if UNITY_EDITOR
			DrakkarEditor.NameAndReasonError("Array.RemoveAt","Index Out of Bounds");
		#endif
			return arr;
		}
	}
}