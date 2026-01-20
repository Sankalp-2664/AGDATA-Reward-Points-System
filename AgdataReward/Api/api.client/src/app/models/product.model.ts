/**
 * Product model for AgData Reward Points System
 * Used across Admin Dashboard, Modals, Services, and API mapping
 */

import { ProductStatus } from './enums';

export interface Product {
    /** Database primary key (Guid as string) */
    id: string;

    /** Business product identifier (e.g. PRD001) - maps to SKU in backend */
    productId: string;

    /** Display name of the product */
    name: string;

    /** Product description */
    description?: string;

    /** Available inventory count */
    stock: number;

    /** Points required to redeem the product */
    points: number;

    /** Product availability status */
    status: ProductStatus;

    /** Product category */
    category?: string;

    /** Product image URL */
    imageUrl?: string;

    /** Optional metadata (future-proofing) */
    createdAt?: string;
    updatedAt?: string;
}

/**
 * Product filter options
 */
export interface ProductFilter {
  status?: ProductStatus | 'All';
  category?: string;
  minPoints?: number;
  maxPoints?: number;
  searchQuery?: string;
  inStock?: boolean;
}
