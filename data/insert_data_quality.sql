--select * from ww_data order by ww_id;
declare
  v_date   date;
  v_weight number;
  v_length number;
begin
  for i in (select ww_id, ww_raw from ww_data order by ww_id) loop
    begin
      v_date   := to_date(substr(i.ww_raw, 0, 10), 'DD.MM.YYYY');
      v_length := instr(i.ww_raw, ' ', 1, 2) - instr(i.ww_raw, ' ');
      if (v_length = 0) then 
        v_weight := to_number(substr(i.ww_raw, instr(i.ww_raw, ' '), v_length));
      else
        v_weight := to_number(substr(i.ww_raw, instr(i.ww_raw, ' ')));
      end if;
      update ww_data set 
        ww_date   = v_date 
       ,ww_weight = v_weight 
      where ww_id = i.ww_id;
      dbms_output.put_line('v_weight: ' || i.ww_raw || ' - ' || instr(i.ww_raw, ' ', 1, 2) || ' - ' || instr(i.ww_raw, ' '));
    exception 
     when others then dbms_output.put_line('Error: ' || i.ww_id);
    end;
  end loop;
exception 
  when others then null;
end;

-- is there a date on the wrong place?
select f1.ww_date as f1_ww_date
      ,f2.ww_date as f2_ww_date
      ,case when f1.ww_date >= f2.ww_date then 'Error' else null end as ww_error
  from ww_data f1
  join ww_data f2
    on f1.ww_id = f2.ww_id - 1
 order by 3;
-- is there any row without a date
select * from ww_data where ww_date is null;
-- is there any row without a weight
select * from ww_data where ww_weight is null;
-- is there a unbelievable weight
select * from ww_data where ww_weight > 111 or ww_weight < 90