import { useParams } from "react-router-dom";
import {
  type GetOrderStatusResult,
  type GetOrderStatusVariables,
} from "../../types/Order";
import { useQuery } from "@apollo/client/react";
import { GET_ORDER_STATUS } from "../../graphql/Orders";

//Displays order status for a given order ID
export default function Confirmation() {
  const { orderid } = useParams();
  const { data, loading, error } = useQuery<
    GetOrderStatusResult,
    GetOrderStatusVariables
  >(GET_ORDER_STATUS, {
    variables: { id: Number(orderid) },
  });

  //Error checking to ensure that data has loaded properly
  if (loading) return <p>Loading order...</p>;
  if (error) return <p>Error loading order</p>;
  if (!data?.orders) return <p>No order found</p>;

  //pull out the first order result from data
  const order = data.orders?.[0];

  //use that order result to populate data fields
  const createdAt = new Date(order.createdAt);
  const pickupTime = new Date(order.pickUpTime);
  return (
    <div>
      <h3>Order number: {order.id}</h3>
      <h1> Hello, {order.customerName}</h1>
      <h2>Your order was placed at {createdAt.toLocaleTimeString()}</h2>
      <h2> Will be ready for pickup at: {pickupTime.toLocaleTimeString()}</h2>
    </div>
  );
}
