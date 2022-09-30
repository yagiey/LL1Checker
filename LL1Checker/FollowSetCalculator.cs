using LL1Checker.Extensions.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1Checker
{
	internal class FollowSetCalculator
	{
		private readonly IDictionary<SymbolSequence, HashSet<Symbol>> _firstSet;
		private readonly IDictionary<Symbol, HashSet<Symbol>> _followSet;
		private readonly IDictionary<Symbol, IEnumerable<SymbolSequence>> _nonTerminalRules;
		private readonly Symbol _startSymbol;

		public FollowSetCalculator(Grammer grammer, IDictionary<SymbolSequence, HashSet<Symbol>> firstSet)
			: this(new Dictionary<Symbol, HashSet<Symbol>>(), grammer, firstSet)
		{
		}

		public FollowSetCalculator(FollowSetCalculator prev, Grammer grammer, IDictionary<SymbolSequence, HashSet<Symbol>> firstSet)
			: this(prev.DeepCopyFollowSet(), grammer, firstSet)
		{
		}

		private FollowSetCalculator(IDictionary<Symbol, HashSet<Symbol>> prevSets, Grammer grammer, IDictionary<SymbolSequence, HashSet<Symbol>> firstSet)
		{
			_firstSet = firstSet;
			_followSet = prevSets;
			_startSymbol = grammer.StartSymbol!;
			_nonTerminalRules = grammer.GetNonTerminalRules();
		}

		public void Calc(Symbol symbol)
		{
			HashSet<Symbol> result = new();
			if (_startSymbol == symbol)
			{
				result.Add(SymbolPool.Eos);
			}

			foreach (var entry in _nonTerminalRules)
			{
				foreach (SymbolSequence seq in entry.Value)
				{
					if (!seq.Any())
					{
						const string ErrMsg = @"The rule which its right hand side is empty sequence was detected.";
						throw new Exception(ErrMsg);
					}

					IEnumerable<Symbol> beta = seq;
					while (true)
					{
						Symbol s = beta.First();
						beta = beta.Skip(1);

						bool isEndOfRest = false;
						if (!beta.Any())
						{
							beta = new Symbol[] { SymbolPool.Empty };
							isEndOfRest = true;
						}

						if (isEndOfRest && s != symbol)
						{
							break;
						}
						else if (s != symbol)
						{
							continue;
						}

						bool containsEmpty = false;
						IEnumerable<Symbol> firstSetOfRest = _firstSet[new SymbolSequence(beta)];
						foreach (Symbol tmp2 in firstSetOfRest)
						{
							if (tmp2 == SymbolPool.Empty)
							{
								containsEmpty = true;
							}
							else
							{
								result.Add(tmp2);
							}
						}

						if (isEndOfRest || containsEmpty)
						{
							if (!_followSet.ContainsKey(entry.Key))
							{
								_followSet.Add(entry.Key, new HashSet<Symbol>());
							}
							HashSet<Symbol> tmp3 = _followSet[entry.Key];
							result.Add(tmp3);
						}

						if (isEndOfRest)
						{
							break;
						}
					}
				}
			}

			Add(symbol, new HashSet<Symbol>(result));
		}

		private void Add(Symbol key, IEnumerable<Symbol> set)
		{
			bool found = _followSet.TryGetValue(key, out HashSet<Symbol>? values);
			if (!found || values is null)
			{
				_followSet.Add(key, new HashSet<Symbol>(set));
			}
			else
			{
				foreach (Symbol value in set)
				{
					_followSet[key].Add(value);
				}
			}
		}

		public IDictionary<Symbol, HashSet<Symbol>> GetFollowSet()
		{
			return _followSet;
		}

		private IDictionary<Symbol, HashSet<Symbol>> DeepCopyFollowSet()
		{
			IDictionary<Symbol, HashSet<Symbol>> clone =
				new Dictionary<Symbol, HashSet<Symbol>>();
			foreach (var entry in _followSet)
			{
				HashSet<Symbol> values = new(entry.Value);
				clone.Add(entry.Key, values);
			}
			return clone;
		}
	}
}
