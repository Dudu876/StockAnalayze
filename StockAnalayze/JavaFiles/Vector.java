package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;

import org.apache.hadoop.io.Text;
import org.apache.hadoop.io.WritableComparable;

public class Vector implements WritableComparable<Vector> {

	private String stockName;
	private double[] vector;

	public Vector() {
		super();
		this.stockName = "";
	}

	public Vector(Vector v) {
		super();
		this.stockName = v.stockName;
		int l = v.vector.length;
		this.vector = new double[l];
		System.arraycopy(v.vector, 0, this.vector, 0, l);
	}
	
	public Vector(Text value){
		super();
		String stringVec = value.toString();
		String[] strArr =  stringVec.split(" ");

		this.stockName = strArr[0];
		
		this.vector = new double[strArr.length - 1];
		
		for (int i = 1; i < strArr.length; i++) {
			this.vector[i - 1] = Double.parseDouble(strArr[i]);
		}
	}
	
	public Vector(double x, double y) {
		super();
		this.stockName = "";
		this.vector = new double[] { x, y };
	}

	@Override
	public void write(DataOutput out) throws IOException {
		out.writeInt(stockName.length());
		out.writeChars(stockName);
		out.writeInt(vector.length);
		for (int i = 0; i < vector.length; i++)
			out.writeDouble(vector[i]);
	}

	@Override
	public void readFields(DataInput in) throws IOException {
		int len = in.readInt();
		this.stockName = "";
		for (int i = 0; i < len; i++){
			stockName += in.readChar();
		}
		int size = in.readInt();
		vector = new double[size];
		for (int i = 0; i < size; i++)
			vector[i] = in.readDouble();
	}

	@Override
	public int compareTo(Vector o) {
		if (this.stockName.compareTo(o.stockName) != 0){
			return this.stockName.compareTo(o.stockName);
		}
		
		if (this.vector.length != o.vector.length) {
			return this.vector.length - o.vector.length;
		}
		
		for (int i = 0; i < vector.length; i++) {
			double c = vector[i] - o.vector[i];
			if (c!= 0.0d)
			{
				return (int)c;
			}		
		}
		return 0;
	}

	public double[] getVector() {
		return vector;
	}

	public void setVector(double[] vector) {
		this.vector = vector;
	}
	
	public String getStockName() {
		return stockName;
	}

	public void setStockName(String stockName) {
		this.stockName = stockName;
	}

	@Override
	public String toString(){
		return this.stockName;
	}
}
