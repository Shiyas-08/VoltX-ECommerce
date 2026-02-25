export type OrderStatus =
  | 'Pending'
  | 'Paid'
  | 'Packed'
  | 'Shipped'
  | 'OutForDelivery'
  | 'Delivered'
  | 'Cancelled';

export interface OrderItem {
  productId: number;
  name: string;
  image: string;
  quantity: number;
  price: number;
  category: string;
}

export interface OrderAddress {
  fullName: string;
  phone: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  pincode: string;
}

export interface Order {
  id: number;
  total: number;
  date: string;
  status: OrderStatus;
  address: OrderAddress;
  items: OrderItem[];
}
