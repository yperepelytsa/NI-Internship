import { Component, Input } from '@angular/core';
import { Item } from './item';
@Component({
  selector: 'my-table',
  template: `  
  <table class="table shop-cart">
  <thead>
  <tr>
  <th style="width:61%"><b class="bg-4">Item name</b></th>
  <th style="width:13%"><b class="bg-4">Price</b></th>
  <th style="width:13%"><b class="bg-4">Quantity</b></th>
  <th style="width:13%"><b class="bg-4">Total</b></th>
  </tr>
  </thead>
  <tbody>       
			<tr *ngFor="let item of itemsinp" class="row-cart-item">
  <td class="vertical-align-center">
<img class="image-preview-card" src={{item.url}}>
<b class="bg-4"> {{item.name}}</b> <button (click)="removeit(item)" class="delete-button"><span class="glyphicon glyphicon-trash"></span></button>
</td>
<td class="vertical-align-center"><p class="bg-4">{{item.itemPrice}}</p></td>
<td class="vertical-align-center"><div class="cart-item-quantity">
<button (click)="decrement(item)" class="quantity-change" type="button">-</button>
<input [(ngModel)]="item.quantity" class="input-quantity" value="{{item.quantity}}"/>
<button (click)="increment(item)" class="quantity-change" type="button">+</button></div></td>
<td class="vertical-align-center"><p class="bg-4">{{item.quantity*item.itemPrice}}</p></td>
</tr>
</tbody>
</table>

  `})
  export class TableComponent {
      @Input()
      itemsinp: Item[];


      decrement(item:Item){
	if(item.quantity>0)
  item.quantity--;
}
removeit(item:Item){
     this.itemsinp.splice(this.itemsinp.indexOf(item),1);
}
increment(item:Item){
  item.quantity++;
}
  }