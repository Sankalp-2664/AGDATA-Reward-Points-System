import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, catchError, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product.model';
import { ProductStatus } from '../models/enums';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/product`;
  
  private productsSubject = new BehaviorSubject<Product[]>([]);
  products$: Observable<Product[]> = this.productsSubject.asObservable();
  
  private rewardPointsSubject = new BehaviorSubject<any[]>([]);
  rewardPoints$: Observable<any[]> = this.rewardPointsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadProducts();
    this.loadRewardPoints();
  }
  
  private loadRewardPoints(): void {
    this.http.get<any[]>('http://localhost:5010/api/reward/points').pipe(
      tap(rewardPoints => {
        console.log('Reward points loaded:', rewardPoints);
        this.rewardPointsSubject.next(rewardPoints);
      }),
      catchError(err => {
        console.error('Error loading reward points:', err);
        return of([]);
      })
    ).subscribe();
  }

  private loadProducts(): void {
    this.http.get<any[]>(`${this.apiUrl}/with-inventory`).pipe(
      tap(products => {
        console.log('Products loaded from backend:', products);
        // Map backend response to frontend model
        const mappedProducts = products.map(p => ({
          id: p.id,
          productId: p.sku, // SKU from backend maps to productId in frontend
          name: p.name,
          stock: p.stock,
          points: p.rewardPointsValue,
          status: p.isActive ? ProductStatus.Active : ProductStatus.Inactive
        }));
        this.productsSubject.next(mappedProducts);
      }),
      catchError(err => {
        console.error('Error loading products:', err);
        // Fallback to mock data if API fails
        this.productsSubject.next([
          {
            id: '1',
            productId: 'PROD001',
            name: 'Laptop',
            stock: 50,
            points: 5000,
            status: ProductStatus.Active
          },
          {
            id: '2',
            productId: 'PROD002',
            name: 'Mouse',
            stock: 30,
            points: 300,
            status: ProductStatus.Active
          },
          {
            id: '3',
            productId: 'PROD003',
            name: 'Mobile',
            stock: 0,
            points: 4000,
            status: ProductStatus.Inactive
          }
        ]);
        return of([]);
      })
    ).subscribe();
  }

  getProducts(): Product[] {
    return this.productsSubject.value;
  }

  addProduct(product: Product, rewardPointsId: string): void {
    // Map frontend model to backend DTO
    const payload = {
      sku: product.productId,
      name: product.name,
      rewardPointsId: rewardPointsId
    };

        this.http.post<any>(`${this.apiUrl}`, payload).pipe(
          tap(createdProduct => {
            console.log('Product created successfully:', createdProduct);
            // If initial stock was specified, update it
            if (product.stock > 0) {
              const productId = createdProduct.id;
              this.http.post(`http://localhost:5010/api/inventory/${productId}/update-stock`, {
                QuantityChange: product.stock
              }).pipe(
                tap(() => {
                  console.log('Initial stock set successfully');
                  this.loadProducts();
                }),
                catchError(err => {
                  console.error('Error setting initial stock:', err);
                  // Still reload products even if stock update fails
                  this.loadProducts();
                  return of(null);
                })
              ).subscribe();
            } else {
              this.loadProducts();
            }
          }),
          catchError(err => {
            console.error('Error creating product:', err);
            alert(err?.error?.message || 'Failed to create product');
            return of(null);
          })
        ).subscribe();
  }

  updateProductStock(productId: string, newStock: number): void {
    // Calculate stock change
    const currentProduct = this.productsSubject.value.find(p => p.id === productId);
    if (!currentProduct) {
      console.error('Product not found:', productId);
      return;
    }
    
    const stockChange = newStock - currentProduct.stock;
    
    // Use inventory API to update stock
    this.http.post(`http://localhost:5010/api/inventory/${productId}/update-stock`, {
      QuantityChange: stockChange
    }).pipe(
      tap(() => {
        console.log('Stock updated successfully');
        // Reload products after stock update
        this.loadProducts();
      }),
      catchError(err => {
        console.error('Error updating stock:', err);
        alert(err?.error?.message || 'Failed to update stock');
        return of(null);
      })
    ).subscribe();
  }

  updateProduct(product: Product, rewardPointsId?: string): void {
    // Map frontend model to backend update DTO
    const payload = {
      id: product.id,
      sku: product.productId,
      name: product.name,
      rewardPointsId: rewardPointsId || null
    };

    this.http.put(`${this.apiUrl}/${product.id}`, payload).pipe(
      tap(() => {
        console.log('Product updated successfully');
        const currentProduct = this.productsSubject.value.find(p => p.id === product.id);
        
        // Update stock if changed
        if (currentProduct && currentProduct.stock !== product.stock) {
          this.updateProductStock(product.id, product.stock);
        }
        
        // Update status if changed
        if (currentProduct && currentProduct.status !== product.status) {
          this.updateProductStatus(product.id, product.status === 'Active');
        }
        
        // Reload products if no other updates needed
        if (currentProduct && 
            currentProduct.stock === product.stock && 
            currentProduct.status === product.status) {
          this.loadProducts();
        }
      }),
      catchError(err => {
        console.error('Error updating product:', err);
        alert(err?.error?.message || 'Failed to update product');
        return of(null);
      })
    ).subscribe();
  }

  updateProductStatus(productId: string, isActive: boolean): void {
    this.http.post(`http://localhost:5010/api/inventory/${productId}/update-status`, {
      IsActive: isActive
    }).pipe(
      tap(() => {
        console.log('Status updated successfully');
        this.loadProducts();
      }),
      catchError(err => {
        console.error('Error updating status:', err);
        alert(err?.error?.message || 'Failed to update status');
        return of(null);
      })
    ).subscribe();
  }

  refreshProducts(): void {
    this.loadProducts();
  }
}