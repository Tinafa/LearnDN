using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public interface IXUIBillBoardCompRef
    {
        IXUISpecLabelSymbol NameSpecLabelSymbol { get; }
        IXUISpecLabelSymbol GuildSpecLabelSymbol { get; }
        IXUISpecLabelSymbol DesiSpecLabelSymbol { get; }
        IXUIProgress BloodBar { get; }
        IXUIProgress IndureBar { get; }
    }
}
