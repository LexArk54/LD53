namespace System {
    public delegate void ActionRef<T1>(ref T1 arg1);
    public delegate void ActionRef<T1, T2>(ref T1 arg1, ref T2 arg2);
    public delegate void ActionRef<T1, T2, T3>(ref T1 arg1, ref T2 arg2, ref T3 arg3);
}