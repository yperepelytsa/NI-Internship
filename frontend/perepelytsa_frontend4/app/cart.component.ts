import { Component, Input } from '@angular/core';
import { Item } from './item';
@Component({
  selector: 'my-cart-value',
  template: `      
			<span id="shopping-cart-badge" class="badge">{{itemsinp.length}}</span>     

  `
})
  export class CartComponent  {
      @Input()
      itemsinp: Item[];

  }