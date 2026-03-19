import { gql } from "@apollo/client";

//Contains GraphQL queries and mutations for getching order data
export const CREATE_ORDER = gql`
  mutation CreateOrder($input: CreateOrderInput!) {
    createOrder(input: $input) {
      id
      customerName
      pizzas {
        quantity
        pizza {
          name
        }
      }
      drinks {
        drinkName
        quantity
      }
      createdAt
      pickUpTime
    }
  }
`;
export const GET_ORDER_STATUS = gql`
  query GetOrderStatus($id: Int!) {
    orders(where: { id: { eq: $id } }) {
      id
      customerName
      createdAt
      pickUpTime
    }
  }
`;