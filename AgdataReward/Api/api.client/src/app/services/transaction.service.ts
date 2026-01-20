import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Transaction, TransactionFilter, TransactionSummary } from '../models/transaction.model';
import { TransactionType } from '../models/enums';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private transactionsSubject = new BehaviorSubject<Transaction[]>([
    {
      id: 1,
      transactionId: 'TXN-2025-001',
      userId: 1,
      userEmployeeId: 'AGD-2024-1523',
      userName: 'Sankalp Chakre',
      type: TransactionType.Earned,
      points: 250,
      description: 'Earned points for Sales Achievement Event - 1st Prize',
      eventId: 1,
      eventName: 'Q1 Sales Achievement',
      createdAt: '2025-01-15T10:30:00',
      createdBy: 'admin'
    },
    {
      id: 2,
      transactionId: 'TXN-2025-002',
      userId: 1,
      userEmployeeId: 'AGD-2024-1523',
      userName: 'Sankalp Chakre',
      type: TransactionType.Redeemed,
      points: -500,
      description: 'Redeemed Amazon Gift Card (500 points)',
      productId: 1,
      productName: 'Amazon Gift Card $50',
      redemptionId: 1,
      createdAt: '2025-01-10T14:20:00'
    },
    {
      id: 3,
      transactionId: 'TXN-2025-003',
      userId: 1,
      userEmployeeId: 'AGD-2024-1523',
      userName: 'Sankalp Chakre',
      type: TransactionType.Earned,
      points: 300,
      description: 'Monthly Performance Bonus',
      eventId: 2,
      eventName: 'Monthly Performance',
      createdAt: '2025-01-05T09:00:00',
      createdBy: 'admin'
    }
  ]);

  transactions$: Observable<Transaction[]> = this.transactionsSubject.asObservable();

  constructor() {}

  getTransactions(): Transaction[] {
    return this.transactionsSubject.value;
  }

  getTransactionsByUserId(userId: number): Transaction[] {
    return this.transactionsSubject.value.filter(t => t.userId === userId);
  }

  getFilteredTransactions(filter: TransactionFilter): Transaction[] {
    let transactions = this.transactionsSubject.value;

    if (filter.type && filter.type !== 'All') {
      transactions = transactions.filter(t => t.type === filter.type);
    }

    if (filter.userId) {
      transactions = transactions.filter(t => t.userId === filter.userId);
    }

    if (filter.startDate) {
      transactions = transactions.filter(t => t.createdAt >= filter.startDate!);
    }

    if (filter.endDate) {
      transactions = transactions.filter(t => t.createdAt <= filter.endDate!);
    }

    if (filter.searchQuery && filter.searchQuery.trim()) {
      const query = filter.searchQuery.toLowerCase();
      transactions = transactions.filter(t =>
        t.description.toLowerCase().includes(query) ||
        t.transactionId.toLowerCase().includes(query) ||
        t.userName.toLowerCase().includes(query)
      );
    }

    return transactions;
  }

  getTransactionSummary(userId?: number): TransactionSummary {
    let transactions = this.transactionsSubject.value;
    
    if (userId) {
      transactions = transactions.filter(t => t.userId === userId);
    }

    const totalEarned = transactions
      .filter(t => t.type === TransactionType.Earned)
      .reduce((sum, t) => sum + t.points, 0);

    const totalRedeemed = Math.abs(
      transactions
        .filter(t => t.type === TransactionType.Redeemed)
        .reduce((sum, t) => sum + t.points, 0)
    );

    const currentBalance = totalEarned - totalRedeemed;

    const now = new Date();
    const thisMonth = now.getMonth();
    const thisYear = now.getFullYear();

    const thisMonthEarned = transactions
      .filter(t => {
        const date = new Date(t.createdAt);
        return t.type === TransactionType.Earned &&
          date.getMonth() === thisMonth &&
          date.getFullYear() === thisYear;
      })
      .reduce((sum, t) => sum + t.points, 0);

    const thisMonthRedeemed = Math.abs(
      transactions
        .filter(t => {
          const date = new Date(t.createdAt);
          return t.type === TransactionType.Redeemed &&
            date.getMonth() === thisMonth &&
            date.getFullYear() === thisYear;
        })
        .reduce((sum, t) => sum + t.points, 0)
    );

    return {
      totalEarned,
      totalRedeemed,
      currentBalance,
      transactionCount: transactions.length,
      thisMonthEarned,
      thisMonthRedeemed
    };
  }

  createTransaction(transaction: Omit<Transaction, 'id' | 'transactionId' | 'createdAt'>): void {
    const transactions = this.transactionsSubject.value;
    const newTransaction: Transaction = {
      ...transaction,
      id: transactions.length + 1,
      transactionId: `TXN-${new Date().getFullYear()}-${(transactions.length + 1).toString().padStart(3, '0')}`,
      createdAt: new Date().toISOString()
    };

    this.transactionsSubject.next([newTransaction, ...transactions]);
  }

  // Mock API call - replace with actual HTTP call
  async fetchTransactions(): Promise<void> {
    // Simulated API call
    console.log('Fetching transactions from API...');
  }
}
