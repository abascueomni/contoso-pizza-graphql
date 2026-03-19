//TypeScript interfaces and types for Order data and related GraphQL responses
export interface OrderResponse {
    id:number;
    customerName: string;
    pizzas: {
        quantity: number;
        pizza: {
            id: number;
            name:string;
            price: number;
        };
    }[];
    drinks:{
        drinkName: string;
        quantity: number;
    }[];
    createdAt:string;
    pickUpTime:string;
}

export interface OrderPizzaInput{
    pizzaId: number;
    quantity: number;
}
export interface DrinkQuantityInput {
    drinkName: string;
    quantity: number;
}

export interface CreateOrderInput {
    customerName: string;
    pizzas: OrderPizzaInput[];
    drinks: DrinkQuantityInput[];
    coupon: string | null;
}

export interface CreateOrderResult {
    createOrder: OrderResponse;
}
export interface CreateOrderVariables{
    input: CreateOrderInput;
}

export interface OrderStatus {
    id:number;
    customerName: string;
    createdAt:string;
    pickUpTime:string;
}
export interface GetOrderStatusResult {
    orders: OrderStatus[];
}
export interface GetOrderStatusVariables {
    id: number;
}