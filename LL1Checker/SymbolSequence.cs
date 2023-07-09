using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LL1Checker
{
	public class SymbolSequence
		: IEnumerable<Symbol>
		, IEquatable<SymbolSequence>
		, IComparable<SymbolSequence>
	{
		private readonly IEnumerable<Symbol> _sequence;

		public SymbolSequence(Symbol symbol)
		{
			_sequence = new Symbol[] { symbol };
		}

		public SymbolSequence(IEnumerable<Symbol> sequence)
		{
			_sequence = sequence;
		}

		public int CompareTo(SymbolSequence? other)
		{
			if (other is null)
			{
				return 1;
			}
			return CompareTo(_sequence, other._sequence);
		}

		public bool Equals(SymbolSequence? other)
		{
			if (other is null)
			{
				return false;
			}
			return Equals(_sequence, other._sequence);
		}

		public override bool Equals(object? obj)
		{
			if (obj is IEnumerable<Symbol> o)
			{
				return Equals(o);
			}
			return false;
		}

		public override int GetHashCode()
		{
			string strIDs = string.Join(",", _sequence.Select(it => it.ID));
			return strIDs.GetHashCode();
		}

		public static bool operator==(SymbolSequence? lhs, SymbolSequence? rhs)
		{
			if (lhs is null && rhs is null)
			{
				return true;
			}
			else if (lhs is null || rhs is null)
			{
				return false;
			}
			else if (ReferenceEquals(lhs, rhs))
			{
				return true;
			}
			else
			{
				return lhs.Equals(rhs);
			}
		}

		public static bool operator !=(SymbolSequence? lhs, SymbolSequence? rhs)
		{
			bool isEq = lhs == rhs;
			return !isEq;
		}

		public override string ToString()
		{
			return string.Join(" ", _sequence.Select(it => it.ToString()));
		}

		public IEnumerator<Symbol> GetEnumerator()
		{
			foreach (Symbol symbol in _sequence)
			{
				yield return symbol;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static int CompareTo(IEnumerable<Symbol> lhs, IEnumerable<Symbol> rhs)
		{
			if (!lhs.Any() && !rhs.Any())
			{
				return 0;
			}
			else if (!lhs.Any())
			{
				return -1;
			}
			else if (!rhs.Any())
			{
				return 1;
			}
			else
			{
				Symbol x = lhs.First();
				Symbol y = rhs.First();
				int result = x.ID.CompareTo(y.ID);
				if (0 == result)
				{
					return CompareTo(lhs.Skip(1), rhs.Skip(1));
				}
				else
				{
					return result;
				}
			}
		}

		private static bool Equals(IEnumerable<Symbol> lhs, IEnumerable<Symbol> rhs)
		{
			if (!lhs.Any() && !rhs.Any())
			{
				return true;
			}
			else if (!lhs.Any() || !rhs.Any())
			{
				return false;
			}
			else
			{
				if (lhs.First() != rhs.First())
				{
					return false;
				}
				else
				{
					return Equals(lhs.Skip(1), rhs.Skip(1));
				}
			}
		}
	}
}
