using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1Checker
{
	internal class EpsilonDerivabilityCalculator
	{
		private readonly HashSet<Symbol> _terminalSymbols;
		private readonly IDictionary<Symbol, IEnumerable<SymbolSequence>> _nonTerminalRules;
		private readonly IDictionary<SymbolSequence, bool> _epsilonDerivability;

		public EpsilonDerivabilityCalculator(Grammer grammer)
			: this(new Dictionary<SymbolSequence, bool>(), grammer)
		{
		}

		public EpsilonDerivabilityCalculator(EpsilonDerivabilityCalculator prev, Grammer grammer)
			:this(prev.DeepCopy(), grammer)
		{
		}

		private EpsilonDerivabilityCalculator(IDictionary<SymbolSequence, bool> clone, Grammer grammer)
		{
			_terminalSymbols = grammer.GetTerminalSymbols();
			_nonTerminalRules = grammer.GetNonTerminalRules();
			_epsilonDerivability = clone;
		}

		private void Add(SymbolSequence key, bool value)
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
			SymbolSequence seq = new(symbol);
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
				IEnumerable<SymbolSequence> rules = _nonTerminalRules[symbol];
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

		public bool Calc(SymbolSequence sequence)
		{
			bool result = sequence.All(it => Calc(it));
			Add(sequence, result);
			return _epsilonDerivability[sequence];
		}

		private IDictionary<SymbolSequence, bool> DeepCopy()
		{
			IDictionary<SymbolSequence, bool> result = new Dictionary<SymbolSequence, bool>();
			foreach (var entry in _epsilonDerivability)
			{
				result.Add(new SymbolSequence(entry.Key), entry.Value);
			}
			return result;
		}

		public IDictionary<SymbolSequence, bool> GetEpsilonDerivability()
		{
			return _epsilonDerivability;
		}
	}
}
