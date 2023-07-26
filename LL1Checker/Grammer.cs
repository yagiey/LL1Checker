using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LL1Checker
{
	public class Grammer<TokenType>
	{
		private readonly IDictionary<Symbol, TokenType?> _terminalRules;
		private readonly IDictionary<Symbol, IList<SymbolList>> _nonTerminalRules;

		public Grammer()
		{
			_terminalRules = new Dictionary<Symbol, TokenType?>();
			_nonTerminalRules = new Dictionary<Symbol, IList<SymbolList>>();
		}

		public Symbol? StartSymbol { get; private set; }

		public void AddRule(Symbol lhs, IEnumerable<Symbol> rhs)
		{
			SymbolList seq = new(rhs);
			bool found = _nonTerminalRules.TryGetValue(lhs, out IList<SymbolList>? after);
			if (!found || after == null)
			{
				_nonTerminalRules.Add(lhs, new List<SymbolList>() { seq });
			}
			else
			{
				if (!after.Any(it => it.SequenceEqual(seq)))
				{
					after.Add(seq);
				}
			}
		}

		public void AddRule(Symbol from, TokenType to)
		{
			_terminalRules.Add(from, to);
		}

		public void SetStartSymbol(Symbol symbol)
		{
			HashSet<Symbol> vocab = GetVocab();
			if (vocab.Any(it => it == symbol))
			{
				StartSymbol = symbol;
				StartSymbol.Rank = 0;
				return;
			}
		}

		public
			Tuple<
				bool,
				IDictionary<SymbolList, HashSet<Symbol>>?,
				IDictionary<Symbol, HashSet<Symbol>>?,
				IDictionary<SymbolList, bool>?,
				IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?,
				IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?
				>
			CheckWhetherLL1OrNot()
		{
			if (StartSymbol is null)
			{
				const string ErrMsg = "start symbol is not specified.";
				Console.WriteLine(ErrMsg);
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)null,
					(IDictionary<Symbol, HashSet<Symbol>>?)null,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}

			if (!CheckSymbols())
			{
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)null,
					(IDictionary<Symbol, HashSet<Symbol>>?)null,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}

			SetSymbolRank();

			////////////////////////////////////////
			// calculate FIRST set.
			////////////////////////////////////////
			IDictionary<SymbolList, HashSet<Symbol>> firstSet;
			double reqTimeFirstSet;
			try
			{
				DateTime bef = DateTime.Now;
				firstSet = GetFirstSet();
				reqTimeFirstSet = DateTime.Now.Subtract(bef).TotalMilliseconds;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("failed to calculate first set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)null,
					(IDictionary<Symbol, HashSet<Symbol>>?)null,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}
			finally { }
			if (firstSet is null || !firstSet.Any())
			{
				Console.WriteLine("failed to calculate first set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)null,
					(IDictionary<Symbol, HashSet<Symbol>>?)null,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}

			////////////////////////////////////////
			// calculate FOLLOW set.
			////////////////////////////////////////
			IDictionary<Symbol, HashSet<Symbol>>? followSet;
			double reqTimeFollowSet;
			try
			{
				DateTime bef = DateTime.Now;
				followSet = GetFollowSet(firstSet);
				reqTimeFollowSet = DateTime.Now.Subtract(bef).TotalMilliseconds;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("failed to calculate follow set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
					(IDictionary<Symbol, HashSet<Symbol>>?)null,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}
			finally { }
			if (followSet is null || !followSet.Any())
			{
				Console.WriteLine("failed to calculate follow set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
					(IDictionary<Symbol, HashSet<Symbol>>?)null,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}

			////////////////////////////////////////
			// check each symbol can derive ε or not
			////////////////////////////////////////
			IDictionary<SymbolList, bool> epsilonDerivabilitySet;
			double reqTimeEpsilonDerivabilitySet;
			try
			{
				DateTime bef = DateTime.Now;
				epsilonDerivabilitySet = GetEpsilonDerivabilitySet(firstSet);
				reqTimeEpsilonDerivabilitySet = DateTime.Now.Subtract(bef).TotalMilliseconds;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("failed to calculate ε-derivability set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
					(IDictionary<Symbol, HashSet<Symbol>>?)followSet,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}
			finally { }
			if (epsilonDerivabilitySet is null || !epsilonDerivabilitySet.Any())
			{
				Console.WriteLine("failed to calculate ε-derivability set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
					(IDictionary<Symbol, HashSet<Symbol>>?)followSet,
					(IDictionary<SymbolList, bool>?)null,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}

			////////////////////////////////////////
			// calculate DIRECTOR set.
			////////////////////////////////////////
			IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>> directorSet;
			double reqTimeDirectorSet;
			try
			{
				DateTime bef = DateTime.Now;
				directorSet = GetDirectorSet(firstSet, followSet, epsilonDerivabilitySet);
				reqTimeDirectorSet = DateTime.Now.Subtract(bef).TotalMilliseconds;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("failed to calculate director set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
					(IDictionary<Symbol, HashSet<Symbol>>?)followSet,
					(IDictionary<SymbolList, bool>?)epsilonDerivabilitySet,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}
			finally { }
			if (directorSet is null || !directorSet.Any())
			{
				Console.WriteLine("failed to calculate director set.");
				return Tuple.Create(
					false,
					(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
					(IDictionary<Symbol, HashSet<Symbol>>?)followSet,
					(IDictionary<SymbolList, bool>?)epsilonDerivabilitySet,
					(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)null,
					(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)null
				);
			}

			////////////////////////////////////////
			// check DIRECTOR set.
			////////////////////////////////////////
			IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>> overwrappingPairsDic;
			double reqTimeCheckDirectorSetOverwrap;
			{
				DateTime bef = DateTime.Now;
				overwrappingPairsDic = CheckDirectorSetOverwrap(directorSet);
				reqTimeCheckDirectorSetOverwrap = DateTime.Now.Subtract(bef).TotalMilliseconds;
			}

			return Tuple.Create(
				!overwrappingPairsDic.Any(),
				(IDictionary<SymbolList, HashSet<Symbol>>?)firstSet,
				(IDictionary<Symbol, HashSet<Symbol>>?)followSet,
				(IDictionary<SymbolList, bool>?)epsilonDerivabilitySet,
				(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>?)directorSet,
				(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>?)overwrappingPairsDic
			);
		}

		private void SetSymbolRank()
		{
			int rank = 0;

			IEnumerable<Symbol> vocab = GetVocab();
			while (true)
			{
				IEnumerable<Symbol> from = vocab.Where(it => it.Rank == rank);
				IEnumerable<Symbol> rest = vocab.Where(it => it.Rank == -1);

				if (!rest.Any())
				{
					break;
				}

				foreach (Symbol symbol in from)
				{
					if (symbol.IsTerminal)
					{
						continue;
					}

					IList<SymbolList> rules = _nonTerminalRules[symbol];
					foreach (SymbolList seq in rules)
					{
						foreach (Symbol s in seq)
						{
							if (s.Rank < 0)
							{
								s.Rank = rank + 1;
							}
						}
					}
				}

				rank++;
			}
		}

		public void DisplayGrammer()
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine(" Grammer");
			Console.WriteLine("----------------------------------------");

			Console.WriteLine("[nonterminal symbols]");
			foreach (Symbol s in _nonTerminalRules.Keys)
			{
				Console.WriteLine($"  {s}");
			}

			Console.WriteLine("[terminal symbols]");
			foreach (Symbol s in _terminalRules.Keys)
			{
				Console.WriteLine($"  {s}");
			}

			Console.WriteLine("[production]");
			foreach (var entry1 in _nonTerminalRules)
			{
				Symbol lhs = entry1.Key;
				foreach (SymbolList rhs in entry1.Value)
				{
					Console.WriteLine($"  {lhs} -> {rhs}");
				}
			}
			Console.WriteLine($"[start symbol]");
			Console.WriteLine($"  {StartSymbol}");
			Console.WriteLine();
		}

		private bool CheckSymbols()
		{
			HashSet<Symbol> vocab = new();
			HashSet<Symbol> nonterm = new();

			foreach (var entry in _terminalRules)
			{
				vocab.Add(entry.Key);
			}

			foreach (var entry in _nonTerminalRules)
			{
				nonterm.Add(entry.Key);
				vocab.Add(entry.Key);
				foreach (IEnumerable<Symbol> seq in entry.Value)
				{
					foreach (Symbol symbol in seq)
					{
						vocab.Add(symbol);
					}
				}
			}

			IEnumerable<Symbol> terms1 = _terminalRules.Keys;
			IEnumerable<Symbol> terms2 =
				vocab
				.Except(nonterm)
				.Where(it => it != SymbolPool.Empty && it != SymbolPool.Eos);

			bool result = terms2.All(it => terms1.Contains(it)) && terms1.All(it => terms2.Contains(it));

			// check terminal symbols
			if (result)
			{
				foreach (Symbol terminalSymbol in _terminalRules.Keys)
				{
					terminalSymbol.IsTerminal = true;
				}
			}

			return result;
		}

		#region FIRST SET

		private IDictionary<SymbolList, HashSet<Symbol>> GetFirstSet()
		{
			SymbolList emptyKey = new(SymbolPool.Empty);
			IDictionary<SymbolList, HashSet<Symbol>> init =
				new Dictionary<SymbolList, HashSet<Symbol>>()
				{
					{ emptyKey,  new HashSet<Symbol>(){ SymbolPool.Empty } }
				};
			IDictionary<SymbolList, bool> done =
				new Dictionary<SymbolList, bool>()
				{
					{ emptyKey, true }
				};

			foreach (Symbol s in _terminalRules.Keys)
			{
				SymbolList key = new(s);
				init.Add(key, new HashSet<Symbol>() { s });
				done.Add(key, true);
			}

			foreach (var entry1 in _nonTerminalRules)
			{

				SymbolList key0 = new(entry1.Key);
				if (!init.ContainsKey(key0))
				{
					init.Add(key0, new HashSet<Symbol>());
				}
				if (!done.ContainsKey(key0))
				{
					done.Add(key0, false);
				}

				foreach (SymbolList seq in entry1.Value)
				{
					IEnumerable<Symbol> tail = seq;
					while (true)
					{
						SymbolList key1 = new(tail);
						if (!init.ContainsKey(key1))
						{
							init.Add(key1, new HashSet<Symbol>());
						}
						if (!done.ContainsKey(key1))
						{
							done.Add(key1, false);
						}

						tail = tail.Skip(1);
						if (!tail.Any())
						{
							break;
						}
					}
				}
			}

			FirstSetCalculator<TokenType> oldSet = new(init, done, this);
			while (true)
			{
				FirstSetCalculator<TokenType> tmp1 = new(oldSet, this);

				foreach (Symbol s in _nonTerminalRules.Keys.OrderBy(it => -it.Rank))
				{
					if (!done[new SymbolList(s)])
					{
						FirstSetCalculator<TokenType> tmp2 = new(tmp1, this);
						tmp2.Calc(s);
						tmp1 = tmp2;
					}
				}

				foreach (var entry1 in _nonTerminalRules)
				{
					foreach (SymbolList seq in entry1.Value)
					{
						IEnumerable<Symbol> tail = seq;
						while (true)
						{
							if (!done[new SymbolList(tail)])
							{
								FirstSetCalculator<TokenType> tmp2 = new(tmp1, this);
								tmp2.Calc(tail);
								tmp1 = tmp2;
							}

							tail = tail.Skip(1);
							if (!tail.Any())
							{
								break;
							}
						}
					}
				}

				if (IsSame(oldSet, tmp1))
				{
					break;
				}
				else
				{
					oldSet = tmp1;
				}
			}

			return oldSet.GetFirstSet();
		}

		private static bool IsSame(FirstSetCalculator<TokenType> lhs, FirstSetCalculator<TokenType> rhs)
		{
			return IsSame(lhs.GetFirstSet(), rhs.GetFirstSet());
		}

		private static bool IsSame(IDictionary<SymbolList, HashSet<Symbol>> lhs, IDictionary<SymbolList, HashSet<Symbol>> rhs)
		{
			return
				lhs.All(it => rhs.ContainsKey(it.Key) && IsSame(rhs[it.Key], it.Value)) &&
				rhs.All(it => lhs.ContainsKey(it.Key) && IsSame(lhs[it.Key], it.Value));
		}

		private static bool IsSame(IEnumerable<Symbol> lhs, IEnumerable<Symbol> rhs)
		{
			return lhs.All(it => rhs.Contains(it)) && rhs.All(it => lhs.Contains(it));
		}

		#endregion

		#region FOLLOW SET

		private IDictionary<Symbol, HashSet<Symbol>> GetFollowSet(IDictionary<SymbolList, HashSet<Symbol>> firstSet)
		{
			FollowSetCalculator<TokenType> oldSet = new(this, firstSet);
			foreach (Symbol s in _nonTerminalRules.Keys.OrderBy(it => it.Rank))
			{
				FollowSetCalculator<TokenType> newSet = new(oldSet, this, firstSet);
				newSet.Calc(s);

				bool allSymbolPrepared = _nonTerminalRules.Keys.All(it => newSet.GetFollowSet().ContainsKey(it));
				if (allSymbolPrepared && IsSame(oldSet, newSet))
				{
					break;
				}

				oldSet = newSet;
			}

			return oldSet.GetFollowSet();
		}

		private static bool IsSame(FollowSetCalculator<TokenType> lhs, FollowSetCalculator<TokenType> rhs)
		{
			return IsSame(lhs.GetFollowSet(), rhs.GetFollowSet());
		}

		private static bool IsSame(IDictionary<Symbol, HashSet<Symbol>> lhs, IDictionary<Symbol, HashSet<Symbol>> rhs)
		{
			return
				lhs.All(it => rhs.ContainsKey(it.Key) && IsSame(rhs[it.Key], it.Value)) &&
				rhs.All(it => lhs.ContainsKey(it.Key) && IsSame(lhs[it.Key], it.Value));
		}

		#endregion

		#region ε-derivability set

		private IDictionary<SymbolList, bool> GetEpsilonDerivabilitySet(IDictionary<SymbolList, HashSet<Symbol>> firstSet)
		{
			EpsilonDerivabilityCalculator<TokenType> oldSet = new(this);
			foreach (SymbolList seq in firstSet.Keys)
			{
				EpsilonDerivabilityCalculator<TokenType> newSet = new(oldSet, this);
				newSet.Calc(seq);
				oldSet = newSet;
			}



			return oldSet.GetEpsilonDerivability();
		}

		#endregion

		#region DIRECTOR SET

		private IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>> GetDirectorSet(IDictionary<SymbolList, HashSet<Symbol>> firstSet, IDictionary<Symbol, HashSet<Symbol>> followSet, IDictionary<SymbolList, bool> epsilonDerivabilitySet)
		{
			if (!firstSet.Any())
			{
				const string ErrMsg = @"The specified first set is empty set.";
				throw new ArgumentException(ErrMsg, nameof(firstSet));
			}
			if (!followSet.Any())
			{
				const string ErrMsg = @"The specified follow set is empty set.";
				throw new ArgumentException(ErrMsg, nameof(followSet));
			}
			if (!epsilonDerivabilitySet.Any())
			{
				const string ErrMsg = @"The specified ε-derivability set is empty set.";
				throw new ArgumentException(ErrMsg, nameof(followSet));
			}

			IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>> directorSet =
				new Dictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>>();
			foreach (var entry in _nonTerminalRules)
			{
				Symbol lhs = entry.Key;
				IDictionary<SymbolList, HashSet<Symbol>> dic = new Dictionary<SymbolList, HashSet<Symbol>>();
				foreach (SymbolList rhs in entry.Value)
				{
					HashSet<Symbol> orign =
						epsilonDerivabilitySet[rhs] ?
						followSet[lhs] :
						firstSet[rhs];
					dic.Add(rhs, new HashSet<Symbol>(orign));
				}
				directorSet.Add(lhs, dic);
			}
			return directorSet;
		}

		#endregion

		#region check DIRECTOR set

		private static IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>
		CheckDirectorSetOverwrap(IDictionary<Symbol, IDictionary<SymbolList, HashSet<Symbol>>> directorSet)
		{
			IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>> overwrappingPairsDic =
				new Dictionary<Symbol, IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>>();

			foreach (var entry in directorSet)
			{
				var overwrappingPairs = GetOverwrappingPairs(directorSet[entry.Key]);
				if (overwrappingPairs.Any())
				{
					overwrappingPairsDic.Add(entry.Key, overwrappingPairs);
				}
			}

			return overwrappingPairsDic;
		}

		private static IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>
		GetOverwrappingPairs(IDictionary<SymbolList, HashSet<Symbol>> sets)
		{
			IList<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>> setPairs =
				new List<Tuple<KeyValuePair<SymbolList, HashSet<Symbol>>, KeyValuePair<SymbolList, HashSet<Symbol>>>>();

			int pos1 = 0;
			foreach (var entry1 in sets)
			{
				HashSet<Symbol> set1 = entry1.Value;
				int pos2 = 0;
				foreach (var entry2 in sets)
				{
					HashSet<Symbol> set2 = entry2.Value;
					if (pos1 <= pos2)
					{
						continue;
					}

					if (set1.Intersect(set2).Any())
					{
						setPairs.Add(Tuple.Create(entry1, entry2));
					}

					pos2++;
				}
				pos1++;
			}
			return setPairs;
		}

		#endregion

		public HashSet<Symbol> GetVocab()
		{
			HashSet<Symbol> result = new(_terminalRules.Keys);
			foreach (var entry in _nonTerminalRules)
			{
				result.Add(entry.Key);
				foreach (IEnumerable<Symbol> seq in entry.Value)
				{
					foreach (Symbol item in seq)
					{
						result.Add(item);
					}
				}
			}
			return result;
		}

		public HashSet<Symbol> GetTerminalSymbols()
		{
			HashSet<Symbol> result = new();
			foreach (var item in _terminalRules.Keys)
			{
				result.Add(item);
			}
			return result;
		}

		public IDictionary<Symbol, IEnumerable<SymbolList>> GetNonTerminalRules()
		{
			IDictionary<Symbol, IEnumerable<SymbolList>> result =
				new Dictionary<Symbol, IEnumerable<SymbolList>>();

			foreach (var entry in _nonTerminalRules)
			{
				IList<SymbolList> values = new List<SymbolList>();
				foreach (IEnumerable<Symbol> sequence in entry.Value)
				{
					SymbolList cloneSeq = new(sequence);
					values.Add(cloneSeq);
				}
				result.Add(entry.Key, values);
			}
			return result;
		}
	}
}
