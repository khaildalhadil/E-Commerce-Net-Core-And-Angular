import { Routes } from '@angular/router';
import { Shop } from './features/shop/shop';
import { UpsertProduct } from './features/shop/upsert-product/upsert-product';

export const routes: Routes = [
	{
		path: "",
		component: Shop
	},
	{
		path: "upsert",
		component: UpsertProduct
	}
];
