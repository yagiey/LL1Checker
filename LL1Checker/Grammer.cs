using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LL1Checker
{
	public class Grammer<TokenType>
	{
		private readonly IDictionary<Symbol, TokenType?> _terminalRules;
		private readonly IDictionary<Symbol, IList<SymbolSequence>> _nonTerminalRules;

		public Grammer()
		{
			_terminalRules = new Dictionary<Symbol, TokenType?>();
			_nonTerminalRules = new Dictionary<Symbol, IList<SymbolSequence>>();
		}

		public Symbol? StartSymbol { get; private set; }

		public void AddRule(Symbol lhs, IEnumerable<Symbol> rhs)
		{
			SymbolSequence seq = new(rhs);
			bool found = _nonTerminalRules.TryGetValue(lhs, out IList<SymbolSequence>? after);
			if (!found || after == null)
			{
				_nonTerminalRules.Add(lhs, new List<SymbolSequence>() { seq });
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
				IDictionary<SymbolSequence, HashSet<Symbol>>?,
				IDictionary<Symbol, HashSet<Symbol>>?,
				IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?
				>
			CheckWhetherLL1OrNot()
		{
			if (StartSymbol is null)
			{
				const string ErrMsg = "start symbol is not specified.";
				Console.WriteLine(ErrMsg);
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)null, (IDictionary<Symbol, HashSet<Symbol>>?)null, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}

			if (!CheckSymbols())
			{
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)null, (IDictionary<Symbol, HashSet<Symbol>>?)null, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}

			DisplayGrammer();
			SetSymbolRank();

			////////////////////////////////////////
			// calculate FIRST set.
			////////////////////////////////////////
			IDictionary<SymbolSequence, HashSet<Symbol>> firstSet;
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
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)null, (IDictionary<Symbol, HashSet<Symbol>>?)null, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			finally { }
			if (firstSet is null || !firstSet.Any())
			{
				Console.WriteLine("failed to calculate first set.");
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)null, (IDictionary<Symbol, HashSet<Symbol>>?)null, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			DisplayFirstSet(firstSet, reqTimeFirstSet);

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
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)null, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			finally { }
			if (followSet is null || !followSet.Any())
			{
				Console.WriteLine("failed to calculate follow set.");
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)null, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			DisplayFollowSet(followSet, reqTimeFollowSet);

			////////////////////////////////////////
			// check each symbol can derive ε or not
			////////////////////////////////////////
			IDictionary<SymbolSequence, bool> epsilonDerivabilitySet;
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
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)followSet, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			finally { }
			if (epsilonDerivabilitySet is null || !epsilonDerivabilitySet.Any())
			{
				Console.WriteLine("failed to calculate ε-derivability set.");
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)followSet, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			DisplayEpsilonDerivabilitySet(epsilonDerivabilitySet, reqTimeEpsilonDerivabilitySet);

			////////////////////////////////////////
			// calculate DIRECTOR set.
			////////////////////////////////////////
			IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>> directorSet;
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
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)followSet, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			finally { }
			if (directorSet is null || !directorSet.Any())
			{
				Console.WriteLine("failed to calculate director set.");
				return Tuple.Create(false, (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)followSet, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)null);
			}
			DisplayDirectorSet(directorSet, reqTimeDirectorSet);

			////////////////////////////////////////
			// check DIRECTOR set.
			////////////////////////////////////////
			IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>> overwrappingPairsDic;
			double reqTimeCheckDirectorSetOverwrap;
			{
				DateTime bef = DateTime.Now;
				overwrappingPairsDic = CheckDirectorSetOverwrap(directorSet);
				reqTimeCheckDirectorSetOverwrap = DateTime.Now.Subtract(bef).TotalMilliseconds;
			}
			DisplayOverwrappingDirectorSets(overwrappingPairsDic, reqTimeCheckDirectorSetOverwrap);

			return Tuple.Create(!overwrappingPairsDic.Any(), (IDictionary<SymbolSequence, HashSet<Symbol>>?)firstSet, (IDictionary<Symbol, HashSet<Symbol>>?)followSet, (IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>?)directorSet);
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

					IList<SymbolSequence> rules = _nonTerminalRules[symbol];
					foreach (SymbolSequence seq in rules)
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

		private void DisplayGrammer()
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
				foreach (SymbolSequence rhs in entry1.Value)
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

		private IDictionary<SymbolSequence, HashSet<Symbol>> GetFirstSet()
		{
			SymbolSequence emptyKey = new(SymbolPool.Empty);
			IDictionary<SymbolSequence, HashSet<Symbol>> init =
				new Dictionary<SymbolSequence, HashSet<Symbol>>()
				{
					{ emptyKey,  new HashSet<Symbol>(){ SymbolPool.Empty } }
				};
			IDictionary<SymbolSequence, bool> done =
				new Dictionary<SymbolSequence, bool>()
				{
					{ emptyKey, true }
				};

			foreach (Symbol s in _terminalRules.Keys)
			{
				SymbolSequence key = new(s);
				init.Add(key, new HashSet<Symbol>() { s });
				done.Add(key, true);
			}

			foreach (var entry1 in _nonTerminalRules)
			{

				SymbolSequence key0 = new(entry1.Key);
				if (!init.ContainsKey(key0))
				{
					init.Add(key0, new HashSet<Symbol>());
				}
				if (!done.ContainsKey(key0))
				{
					done.Add(key0, false);
				}

				foreach (SymbolSequence seq in entry1.Value)
				{
					IEnumerable<Symbol> tail = seq;
					while (true)
					{
						SymbolSequence key1 = new(tail);
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
					if (!done[new SymbolSequence(s)])
					{
						FirstSetCalculator<TokenType> tmp2 = new(tmp1, this);
						tmp2.Calc(s);
						tmp1 = tmp2;
					}
				}

				foreach (var entry1 in _nonTerminalRules)
				{
					foreach (SymbolSequence seq in entry1.Value)
					{
						IEnumerable<Symbol> tail = seq;
						while (true)
						{
							if (!done[new SymbolSequence(tail)])
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

		private static bool IsSame(IDictionary<SymbolSequence, HashSet<Symbol>> lhs, IDictionary<SymbolSequence, HashSet<Symbol>> rhs)
		{
			return
				lhs.All(it => rhs.ContainsKey(it.Key) && IsSame(rhs[it.Key], it.Value)) &&
				rhs.All(it => lhs.ContainsKey(it.Key) && IsSame(lhs[it.Key], it.Value));
		}

		private static bool IsSame(IEnumerable<Symbol> lhs, IEnumerable<Symbol> rhs)
		{
			return lhs.All(it => rhs.Contains(it)) && rhs.All(it => lhs.Contains(it));
		}

		private static void DisplayFirstSet(IDictionary<SymbolSequence, HashSet<Symbol>> firstSet, double requiredTime)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine($"FIRST SET : required time={requiredTime} ms");
			Console.WriteLine("----------------------------------------");
			foreach (var entry in firstSet)
			{
				string strValue = string.Join(" ", entry.Value);
				Console.WriteLine($"FIRST([{entry.Key}]) -> {{{strValue}}}");
			}
			Console.WriteLine();
		}

		#endregion

		#region FOLLOW SET

		private IDictionary<Symbol, HashSet<Symbol>> GetFollowSet(IDictionary<SymbolSequence, HashSet<Symbol>> firstSet)
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

		private static void DisplayFollowSet(IDictionary<Symbol, HashSet<Symbol>> followSet, double requiredTime)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine($"FOLLOW SET : required time={requiredTime} ms");
			Console.WriteLine("----------------------------------------");
			foreach (var entry in followSet)
			{
				string strValue = string.Join(" ", entry.Value);
				Console.WriteLine($"FOLLOW({entry.Key}) -> {{{strValue}}}");
			}
			Console.WriteLine();
		}

		#endregion

		#region ε-derivability set

		private IDictionary<SymbolSequence, bool> GetEpsilonDerivabilitySet(IDictionary<SymbolSequence, HashSet<Symbol>> firstSet)
		{
			EpsilonDerivabilityCalculator<TokenType> oldSet = new(this);
			foreach (SymbolSequence seq in firstSet.Keys)
			{
				EpsilonDerivabilityCalculator<TokenType> newSet = new(oldSet, this);
				newSet.Calc(seq);
				oldSet = newSet;
			}



			return oldSet.GetEpsilonDerivability();
		}

		private static void DisplayEpsilonDerivabilitySet(IDictionary<SymbolSequence, bool> epsilonDerivabilitySet, double requiredTime)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine($"ε-Derivability SET : required time={requiredTime} ms");
			Console.WriteLine("----------------------------------------");
			foreach (var entry in epsilonDerivabilitySet)
			{
				Console.WriteLine($"εDerivability([{entry.Key}]) -> {entry.Value}");
			}
			Console.WriteLine();
		}

		#endregion

		#region DIRECTOR SET

		private IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>> GetDirectorSet(IDictionary<SymbolSequence, HashSet<Symbol>> firstSet, IDictionary<Symbol, HashSet<Symbol>> followSet, IDictionary<SymbolSequence, bool> epsilonDerivabilitySet)
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

			IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>> directorSet =
				new Dictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>>();
			foreach (var entry in _nonTerminalRules)
			{
				Symbol lhs = entry.Key;
				IDictionary<SymbolSequence, HashSet<Symbol>> dic = new Dictionary<SymbolSequence, HashSet<Symbol>>();
				foreach (SymbolSequence rhs in entry.Value)
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

		private static void DisplayDirectorSet(IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>> directorSet, double requiredTime)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine($"DIRECTOR SET : required time={requiredTime} ms");
			Console.WriteLine("----------------------------------------");
			foreach (var entry1 in directorSet)
			{
				foreach (var entry2 in entry1.Value)
				{
					string strSet = string.Join(" ", entry2.Value);
					Console.WriteLine($"DIRECTOR({entry1.Key},[{entry2.Key}]) -> {{{strSet}}}");
				}
			}
			Console.WriteLine();
		}

		#endregion

		#region check DIRECTOR set

		private static IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>>
		CheckDirectorSetOverwrap(IDictionary<Symbol, IDictionary<SymbolSequence, HashSet<Symbol>>> directorSet)
		{
			IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>> overwrappingPairsDic =
				new Dictionary<Symbol, IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>>();

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

		private static IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>
		GetOverwrappingPairs(IDictionary<SymbolSequence, HashSet<Symbol>> sets)
		{
			IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>> setPairs =
				new List<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>();

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

		private static void DisplayOverwrappingDirectorSets(IDictionary<Symbol, IList<Tuple<KeyValuePair<SymbolSequence, HashSet<Symbol>>, KeyValuePair<SymbolSequence, HashSet<Symbol>>>>> overwrappingPairsDic, double requiredTime)
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine($"Overwrapping DIRECTOR SET : required time={requiredTime} ms");
			Console.WriteLine("----------------------------------------");

			if (overwrappingPairsDic.Count <= 0)
			{
				Console.WriteLine("Theres is no overwrapping Director set");
				Console.WriteLine();
				return;
			}

			foreach (var entry in overwrappingPairsDic)
			{
				Symbol param1 = entry.Key;
				foreach (var pair in entry.Value)
				{
					string strSet1 = string.Join(" ", pair.Item1.Value);
					Console.WriteLine($"DIRECTOR({param1}, [{pair.Item1.Key}]) -> {{{strSet1}}}");
					string strSet2 = string.Join(" ", pair.Item2.Value);
					Console.WriteLine($"DIRECTOR({param1}, [{pair.Item2.Key}]) -> {{{strSet2}}}");
				}
			}
			Console.WriteLine();
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

		public IDictionary<Symbol, IEnumerable<SymbolSequence>> GetNonTerminalRules()
		{
			IDictionary<Symbol, IEnumerable<SymbolSequence>> result =
				new Dictionary<Symbol, IEnumerable<SymbolSequence>>();

			foreach (var entry in _nonTerminalRules)
			{
				IList<SymbolSequence> values = new List<SymbolSequence>();
				foreach (IEnumerable<Symbol> sequence in entry.Value)
				{
					SymbolSequence cloneSeq = new(sequence);
					values.Add(cloneSeq);
				}
				result.Add(entry.Key, values);
			}
			return result;
		}
	}
}
