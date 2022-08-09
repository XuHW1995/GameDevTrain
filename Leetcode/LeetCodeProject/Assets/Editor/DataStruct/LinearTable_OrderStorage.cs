/******************************************************************************
* Created by: XuHongWei
* Date: 2022-08-09 22:18:28
* Des: 自定义线性表，顺序存储
*******************************************************************************/

public class LinearTable<TItem>
{
    private TItem[] _items;
    public int Length
    {
        get { return _items.Length; }
    }
    
    public void Insert(int index, TItem item)
    {
        
    }

    public void Delete(int index)
    {
        
    }
    
    public TItem FindItem(int index)
    {
        return _items[index];
    }

    public int FindItemFirstIndex(TItem item)
    {
        return 0;
    }
}
