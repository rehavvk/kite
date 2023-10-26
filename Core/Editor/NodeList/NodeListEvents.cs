using System;

namespace Rehawk.Kite.NodeList
{
    public sealed class ItemClickedEventArgs : EventArgs
    {
        public NodeListAdaptor Adaptor { get; }

        public int ItemIndex { get; }

        public ItemClickedEventArgs(NodeListAdaptor adaptor, int itemIndex)
        {
            this.Adaptor = adaptor;
            this.ItemIndex = itemIndex;
        }
    }

    public delegate void ItemClickedEventHandler(object sender, ItemClickedEventArgs args);
}