using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1Checker
{
	internal class FirstSetCalculator<TokenType>
	{
		private readonly IDictionary<SymbolSequence, HashSet<Symbol>> _firstSet;
		private readonly IDictionary<SymbolSequence, bool> _done;
		private readonly HashSet<Symbol> _terminalSymbols;
		private readonly IDictionary<Symbol, IEnumerable<SymbolSequence>> _nonTerminalRules;

		public FirstSetCalculator(FirstSetCalculator<TokenType> prev, Grammer<TokenType> grammer)
			: this(prev.DeepCopyFirstSet(), prev._done, grammer)
		{
		}

		public FirstSetCalculator(IDictionary<SymbolSequence, HashSet<Symbol>> prevSets, IDictionary<SymbolSequence, bool> done, Grammer<TokenType> grammer)
		{
			_firstSet = prevSets;
			_done = done;
			_terminalSymbols = grammer.GetTerminalSymbols();
			_nonTerminalRules = grammer.GetNonTerminalRules();
		}

		public void Calc(Symbol symbol)
		{
			SymbolSequence key = new(symbol);
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
				IEnumerable<SymbolSequence> rules = _nonTerminalRules[symbol];
				HashSet<Symbol> result = new();
				bool isDone1 = true;
				foreach (SymbolSequence rule in rules)
				{
					if (!_done[rule])
					{
						isDone1 = false;
					}

					foreach (Symbol s in _firstSet[rule])
					{
						SymbolSequence key1 = new(s);
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

		public void Calc(IEnumerable<Symbol> sequence)
		{
			if (!sequence.Any())
			{
				const string ErrMsg = @"The parameter 'sequence' must have at least one element.";
				throw new ArgumentException(ErrMsg, nameof(sequence));
			}

			Symbol head = sequence.First();
			SymbolSequence keyHead = new(head);
			HashSet<Symbol> firstOfHead = new(_firstSet[keyHead]);
			SymbolSequence key2 = new(sequence);

			IEnumerable<Symbol> rest = sequence.Skip(1);
			if (rest.Any() && firstOfHead.Contains(SymbolPool.Empty))
			{
				SymbolSequence keyRest = new(rest);
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

		private void Add(SymbolSequence key, Symbol value)
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

		private void Add(SymbolSequence key, IEnumerable<Symbol> set)
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

		public IDictionary<SymbolSequence, HashSet<Symbol>> GetFirstSet()
		{
			return _firstSet;
		}

		private IDictionary<SymbolSequence, HashSet<Symbol>> DeepCopyFirstSet()
		{
			IDictionary<SymbolSequence, HashSet<Symbol>> clone
				= new Dictionary<SymbolSequence, HashSet<Symbol>>();

			foreach (var entry in _firstSet)
			{
				SymbolSequence key = new(entry.Key);
				HashSet<Symbol> values = new(entry.Value);
				clone.Add(key, values);
			}
			return clone;
		}
	}
}
