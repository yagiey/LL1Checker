using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1Checker
{
	internal class EpsilonDerivabilityCalculator<TokenType>
	{
		private readonly HashSet<Symbol> _terminalSymbols;
		private readonly IDictionary<Symbol, IEnumerable<SymbolList>> _nonTerminalRules;
		private readonly IDictionary<SymbolList, bool> _epsilonDerivability;

		public EpsilonDerivabilityCalculator(Grammer<TokenType> grammer)
			: this(new Dictionary<SymbolList, bool>(), grammer)
		{
		}

		public EpsilonDerivabilityCalculator(EpsilonDerivabilityCalculator<TokenType> prev, Grammer<TokenType> grammer)
			:this(prev.DeepCopy(), grammer)
		{
		}

		private EpsilonDerivabilityCalculator(IDictionary<SymbolList, bool> clone, Grammer<TokenType> grammer)
		{
			_terminalSymbols = grammer.GetTerminalSymbols();
			_nonTerminalRules = grammer.GetNonTerminalRules();
			_epsilonDerivability = clone;
		}

		private void Add(SymbolList key, bool value)
		{
			bool found = _epsilonDerivability.TryGetValue(key, out _);
			if (!found)
			{
				_epsilonDerivability.Add(key, value);
			}
			else if (value)
			{
				_epsilonDerivability[key] = value;
			}
		}

		public bool Calc(Symbol symbol)
		{
			SymbolList seq = new(symbol);
			if (_terminalSymbols.Contains(symbol))
			{
				////////////////////////////////////////
				// terminal symbol
				////////////////////////////////////////
				Add(seq, false);
				return _epsilonDerivability[seq];
			}
			else if (symbol.IsEmpty())
			{
				////////////////////////////////////////
				// ε
				////////////////////////////////////////
				Add(seq, true);
				return _epsilonDerivability[seq];
			}
			else
			{
				////////////////////////////////////////
				// nonterminal symbol
				////////////////////////////////////////
				IEnumerable<SymbolList> rules = _nonTerminalRules[symbol];
				bool result = rules.Where(it => !it.Contains(symbol)).Any(it => Calc(it));
				Add(seq, result);

				// prevent enternal recursion
				foreach (var item in rules.Where(it => it.Contains(symbol)))
				{
					Add(item, false);
				}

				return _epsilonDerivability[seq];
			}
		}

		public bool Calc(SymbolList symbolList)
		{
			bool result = symbolList.All(it => Calc(it));
			Add(symbolList, result);
			return _epsilonDerivability[symbolList];
		}

		private IDictionary<SymbolList, bool> DeepCopy()
		{
			IDictionary<SymbolList, bool> result = new Dictionary<SymbolList, bool>();
			foreach (var entry in _epsilonDerivability)
			{
				result.Add(new SymbolList(entry.Key), entry.Value);
			}
			return result;
		}

		public IDictionary<SymbolList, bool> GetEpsilonDerivability()
		{
			return _epsilonDerivability;
		}
	}
}
