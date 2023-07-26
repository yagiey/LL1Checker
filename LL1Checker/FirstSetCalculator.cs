using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1Checker
{
	internal class FirstSetCalculator<TokenType>
	{
		private readonly IDictionary<SymbolList, HashSet<Symbol>> _firstSet;
		private readonly IDictionary<SymbolList, bool> _done;
		private readonly HashSet<Symbol> _terminalSymbols;
		private readonly IDictionary<Symbol, IEnumerable<SymbolList>> _nonTerminalRules;

		public FirstSetCalculator(FirstSetCalculator<TokenType> prev, Grammer<TokenType> grammer)
			: this(prev.DeepCopyFirstSet(), prev._done, grammer)
		{
		}

		public FirstSetCalculator(IDictionary<SymbolList, HashSet<Symbol>> prevSets, IDictionary<SymbolList, bool> done, Grammer<TokenType> grammer)
		{
			_firstSet = prevSets;
			_done = done;
			_terminalSymbols = grammer.GetTerminalSymbols();
			_nonTerminalRules = grammer.GetNonTerminalRules();
		}

		public void Calc(Symbol symbol)
		{
			SymbolList key = new(symbol);
			if (symbol.IsEmpty() || _terminalSymbols.Contains(symbol))
			{
				//////////////////////////////
				// symbol is ε or terminal symbol
				//////////////////////////////
				Add(key, symbol);
				return;
			}
			else
			{
				//////////////////////////////
				// symbol is non-terminal symbol
				//////////////////////////////
				IEnumerable<SymbolList> rules = _nonTerminalRules[symbol];
				HashSet<Symbol> result = new();
				bool isDone1 = true;
				foreach (SymbolList rule in rules)
				{
					if (!_done[rule])
					{
						isDone1 = false;
					}

					foreach (Symbol s in _firstSet[rule])
					{
						SymbolList key1 = new(s);
						if (!_done[key1])
						{
							isDone1 = false;
						}
						result.Add(s);
					}
				}
				_done[key] = isDone1;
				Add(key, result);
				return;
			}
		}

		public void Calc(IEnumerable<Symbol> symbolList)
		{
			if (!symbolList.Any())
			{
				const string ErrMsg = @"The parameter 'symbolList' must have at least one element.";
				throw new ArgumentException(ErrMsg, nameof(symbolList));
			}

			Symbol head = symbolList.First();
			SymbolList keyHead = new(head);
			HashSet<Symbol> firstOfHead = new(_firstSet[keyHead]);
			SymbolList key2 = new(symbolList);

			IEnumerable<Symbol> rest = symbolList.Skip(1);
			if (rest.Any() && firstOfHead.Contains(SymbolPool.Empty))
			{
				SymbolList keyRest = new(rest);
				HashSet<Symbol> firstOfRest = new(_firstSet[keyRest]);
				firstOfHead.Remove(SymbolPool.Empty);
				IEnumerable<Symbol> result = firstOfHead.Concat(firstOfRest);

				Add(key2, result);
				if (_done[keyHead] && _done[keyRest])
				{
					_done[key2] = true;
				}
				return;
			}
			else
			{
				Add(key2, firstOfHead);
				if (_done[keyHead])
				{
					_done[key2] = true;
				}
				return;
			}
		}

		private void Add(SymbolList key, Symbol value)
		{
			bool found = _firstSet.TryGetValue(key, out HashSet<Symbol>? values);
			if (!found || values is null)
			{
				_firstSet.Add(key, new HashSet<Symbol> { value });
			}
			else
			{
				_firstSet[key].Add(value);
			}
		}

		private void Add(SymbolList key, IEnumerable<Symbol> set)
		{
			bool found = _firstSet.TryGetValue(key, out HashSet<Symbol>? values);
			if (!found || values is null)
			{
				_firstSet.Add(key, new HashSet<Symbol>(set));
			}
			else
			{
				foreach (Symbol value in set)
				{
					_firstSet[key].Add(value);
				}
			}
		}

		public IDictionary<SymbolList, HashSet<Symbol>> GetFirstSet()
		{
			return _firstSet;
		}

		private IDictionary<SymbolList, HashSet<Symbol>> DeepCopyFirstSet()
		{
			IDictionary<SymbolList, HashSet<Symbol>> clone
				= new Dictionary<SymbolList, HashSet<Symbol>>();

			foreach (var entry in _firstSet)
			{
				SymbolList key = new(entry.Key);
				HashSet<Symbol> values = new(entry.Value);
				clone.Add(key, values);
			}
			return clone;
		}
	}
}
